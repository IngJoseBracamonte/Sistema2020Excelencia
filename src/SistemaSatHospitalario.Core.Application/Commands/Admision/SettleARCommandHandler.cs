using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SettleARCommandHandler : IRequestHandler<SettleARCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public SettleARCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SettleARCommand request, CancellationToken cancellationToken)
        {
            // 0. SENIOR CLEANSE: Eliminamos cualquier rastro de tracking previo para evitar side-effects de concurrencia
            _context.ChangeTracker.Clear();

            // 1. CARGA AISLADA: Todo con AsNoTracking para que EF no intente "vigilar" ni "actualizar" estos objetos accidentalmente
            var ar = await _context.CuentasPorCobrar
                .AsNoTracking()
                .Include(a => a.Cuenta)
                .FirstOrDefaultAsync(a => a.Id == request.ArId, cancellationToken);

            if (ar == null) 
                throw new Exception($"Cuenta por cobrar con ID {request.ArId} no fue encontrada.");
            
            if (ar.Estado == EstadoConstants.Cobrada) 
                return true;

            var tasaActual = await _context.TasaCambio
                .AsNoTracking()
                .Where(t => t.Activo)
                .OrderByDescending(t => t.Fecha)
                .FirstOrDefaultAsync(cancellationToken);

            if (tasaActual == null || tasaActual.Monto <= 0)
                throw new Exception("No existe una tasa de cambio activa configurada.");

            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            try
            {
                var recibo = await _context.RecibosFactura
                    .FirstOrDefaultAsync(r => r.CuentaServicioId == ar.CuentaServicioId && r.EstadoFiscal == EstadoConstants.Borrador, cancellationToken);

                Guid reciboId;
                if (recibo == null)
                {
                    var nuevoRecibo = new ReciboFactura(ar.CuentaServicioId, ar.PacienteId, null, tasaActual.Monto, ar.MontoTotalBase);
                    _context.RecibosFactura.Add(nuevoRecibo);
                    reciboId = nuevoRecibo.Id;
                }
                else
                {
                    reciboId = recibo.Id;
                }

                decimal totalAbonadoUSD = 0;
                foreach (var payment in request.Payments)
                {
                    var amountUSD = Math.Round(payment.Amount, 2);
                    totalAbonadoUSD += amountUSD;

                    var detalle = new DetallePago(reciboId, payment.Method, payment.Reference, payment.AmountMoneda, amountUSD);
                    _context.DetallesPago.Add(detalle);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Cálculo de nuevo saldo (Atómico e Independiente del Tracker)
                decimal nuevoMontoPagado = ar.MontoPagadoBase + totalAbonadoUSD;
                string nuevoEstado = nuevoMontoPagado >= ar.MontoTotalBase ? EstadoConstants.Cobrada : EstadoConstants.Parcial;

                var rowsAffected = await _context.CuentasPorCobrar
                    .Where(a => a.Id == request.ArId && a.Estado != EstadoConstants.Cobrada)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(a => a.Estado, nuevoEstado)
                        .SetProperty(a => a.MontoPagadoBase, nuevoMontoPagado), 
                    cancellationToken);

                // Si rowsAffected es 0, puede ser porque ya estaba Cobrada (reintento). No fallamos si ya está en el estado deseado.
                if (rowsAffected == 0)
                {
                    var checkAr = await _context.CuentasPorCobrar.AsNoTracking().FirstOrDefaultAsync(a => a.Id == request.ArId, cancellationToken);
                    if (checkAr?.Estado != EstadoConstants.Cobrada && checkAr?.Estado != EstadoConstants.Parcial)
                    {
                        throw new DbUpdateConcurrencyException("No se pudo actualizar el saldo de la cuenta. Verifique si el estado cambió simultáneamente.");
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception($"Fallo crítico en el motor de liquidación: {ex.Message}", ex);
            }
        }

    }
}
