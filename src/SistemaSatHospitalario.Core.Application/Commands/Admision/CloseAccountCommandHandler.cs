using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using System.Text.RegularExpressions;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly ICajaAdministrativaRepository _cajaRepository;
        private readonly IBillingRepository _billingRepository;

        public CloseAccountCommandHandler(
            IApplicationDbContext context, 
            ILegacyLabRepository legacyRepository,
            ICajaAdministrativaRepository cajaRepository,
            IBillingRepository billingRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
            _cajaRepository = cajaRepository;
            _billingRepository = billingRepository;
        }

        public async Task<Guid> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            if (cuenta == null) throw new Exception("Cuenta no encontrada.");
            if (cuenta.Estado != EstadoConstants.Abierta) throw new Exception("La cuenta ya ha sido procesada.");

            // 0. Gestión Automática de Caja (Micro-Ciclo 28)
            // Si el usuario no tiene una caja abierta, la abrimos automáticamente
            var caja = await _cajaRepository.ObtenerCajaAbiertaPorUsuarioAsync(request.UsuarioId, cancellationToken);
            if (caja == null)
            {
                caja = new CajaDiaria(0, 0, request.UsuarioId, request.UsuarioCajero);
                await _cajaRepository.AgregarCajaAsync(caja, cancellationToken);
                // El guardado se delega al SaveChangesAsync del final del handler
            }

            // 1. Crear el Recibo/Factura vinculado a la caja (automática o existente)
            decimal totalCuenta = cuenta.CalcularTotal();
            decimal totalPagado = request.Pagos.Sum(p => p.EquivalenteAbonadoBase);
            decimal montoVueltoUSD = Math.Max(0, totalPagado - totalCuenta);

            var recibo = new ReciboFactura(cuenta.Id, cuenta.PacienteId, caja.Id, request.TasaCambio, totalCuenta, montoVueltoUSD);
            foreach (var p in request.Pagos)
            {
                recibo.AgregarDetallePago(p.MetodoPago, p.ReferenciaBancaria, p.MontoAbonadoMoneda, p.EquivalenteAbonadoBase);
            }

            // 2. Gestionar Saldo Pendiente (AR)
            // decimal totalCuenta = cuenta.CalcularTotal(); // Ya calculado arriba
            // decimal totalPagado = recibo.ObtenerTotalPagadoBase(); // Ya calculado arriba

            // 4. Finalizar Cuenta (V10.9 SQL Direct Fix)
            // Usamos SQL Directo para puentear el Change Tracker de EF y evitar el error "Affected 0 rows"
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            try 
            {
                // 4.1 Validamos existencia física (AsNoTracking para frescura total)
                var existe = await _context.CuentasServicios
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == request.CuentaId && c.Estado == EstadoConstants.Abierta, cancellationToken);
                
                if (!existe) throw new Exception("La cuenta no existe o ya fue procesada.");

                // 4.2 Lógica de Negocio (AR y Legacy usando la cuenta original cargada al inicio)
                if (totalPagado < totalCuenta)
                {
                    var ar = new CuentaPorCobrar(cuenta.Id, cuenta.PacienteId, totalCuenta, totalPagado);
                    _context.CuentasPorCobrar.Add(ar);
                }

                // Extraemos items para pasarlos después
                var itemsLab = cuenta.Detalles.Where(d => EstadoConstants.EsLaboratorio(d.TipoServicio)).ToList();

                // 4.3 EJECUCIÓN NUCLEAR: Actualización Directa en DB vía Repositorio
                // Previene DbUpdateConcurrencyException (V10.9 SQL Direct)
                await _billingRepository.ForzarCierreCuentaAsync(request.CuentaId, DateTime.UtcNow, cancellationToken);

                // 4.4 Persistimos los demás objetos locales (Recibo, AR)
                _context.RecibosFactura.Add(recibo);
                await _context.SaveChangesAsync(cancellationToken);
                
                // 4.5 EJECUCIÓN LEGACY: Justo antes del Commit local para prevenir órdenes huérfanas en MySQL si lo local revienta
                if (itemsLab.Any())
                {
                    await ProcessLegacyOrder(cuenta, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception($"Error crítico en el cierre de cuenta (V10.9): {ex.Message}", ex);
            }

            return recibo.Id;
        }

        private async Task ProcessLegacyOrder(CuentaServicios cuenta, CancellationToken ct)
        {
            // Senior Logic: Sincronización basado en IDs persistentes (V12.1 ID-First)
            var paciente = await _context.PacientesAdmision
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == cuenta.PacienteId, ct);

            if (paciente == null || !paciente.IdPacienteLegacy.HasValue || paciente.IdPacienteLegacy.Value == 0) return;

            int legacyId = paciente.IdPacienteLegacy.Value;

            var labItems = cuenta.Detalles
                .Where(d => EstadoConstants.EsLaboratorio(d.TipoServicio) && !string.IsNullOrEmpty(d.LegacyMappingId))
                .ToList();

            if (!labItems.Any()) return;

            var perfilesFacturados = new List<PerfilesFacturadosLegacy>();
            foreach (var item in labItems)
            {
                if (int.TryParse(item.LegacyMappingId, out int idPerfil))
                {
                    perfilesFacturados.Add(new PerfilesFacturadosLegacy 
                    { 
                        IdOrden = 0, // Se asigna en el repositorio
                        IdPersona = legacyId,
                        IdPerfil = idPerfil, 
                        PrecioPerfil = item.Precio * item.Cantidad 
                    });
                }
            }

            if (!perfilesFacturados.Any()) return;

            var orden = new OrdenLegacy
            {
                IdPersona = legacyId,
                IDConvenio = cuenta.ConvenioId ?? 1,
                Fecha = DateTime.Now,
                HoraIngreso = DateTime.Now.ToString("HH:mm:ss")
            };

            await _legacyRepository.GenerarOrdenLaboratorioAsync(orden, perfilesFacturados, new List<ResultadosPacienteLegacy>(), ct);
        }
    }
}
