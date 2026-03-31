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
            var ar = await _context.CuentasPorCobrar
                .Include(a => a.Cuenta)
                .FirstOrDefaultAsync(a => a.Id == request.ArId, cancellationToken);

            if (ar == null) throw new Exception("Cuenta por cobrar no encontrada.");
            if (ar.Estado == EstadoConstants.Cobrada) throw new Exception("Esta cuenta ya ha sido cobrada.");

            // Obtener la tasa de cambio activa
            var tasaActual = await _context.TasaCambio
                .Where(t => t.Activo)
                .OrderByDescending(t => t.Fecha)
                .FirstOrDefaultAsync(cancellationToken);

            decimal tasaValue = tasaActual?.Monto ?? throw new Exception("No existe una tasa de cambio activa configurada.");

            // Buscar el recibo asociado a esta cuenta para anexar los pagos
            var recibo = await _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .OrderByDescending(r => r.FechaEmision)
                .FirstOrDefaultAsync(r => r.CuentaServicioId == ar.CuentaServicioId, cancellationToken);
            
            // Si no existe un recibo (borrador o emitido), creamos uno administrativo para registrar estos pagos
            if (recibo == null)
            {
                recibo = new ReciboFactura(ar.CuentaServicioId, ar.PacienteId, null, tasaValue, ar.MontoTotalBase);
                _context.RecibosFactura.Add(recibo);
            }

            // Registrar cada pago en el detalle
            foreach (var payment in request.Payments)
            {
                // El monto base viene normalizado desde el frontend como USD ($)
                var amountUSD = Math.Round(payment.Amount, 2);

                recibo.AgregarDetallePago(
                    payment.Method, 
                    payment.Reference, 
                    payment.AmountMoneda, 
                    amountUSD); 
            }

            // Liquidar la cuenta por cobrar
            ar.MarcarComoCobrada();

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
