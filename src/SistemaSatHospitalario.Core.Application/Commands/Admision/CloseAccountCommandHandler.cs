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
                    await ProcessLegacyOrder(request.UsuarioId, cuenta, itemsLab, cancellationToken);
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

        private async Task ProcessLegacyOrder(string usuarioLocal, CuentaServicios cuenta, List<DetalleServicioCuenta> items, CancellationToken cancellationToken)
        {
            // V11.0: Recuperamos el ID del legado desde la entidad maestra del paciente
            var paciente = await _context.PacientesAdmision
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == cuenta.PacienteId, cancellationToken);
            
            int idPersonaLegacy = paciente?.IdPacienteLegacy ?? 0;
            if (idPersonaLegacy == 0) return; // No hay vínculo legacy

            var idsServicios = items.Select(i => i.ServicioId).Distinct().ToList();
            var serviciosInfo = await _context.ServiciosClinicos
                .Where(s => idsServicios.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Codigo, cancellationToken);

            var orden = new OrdenLegacy 
            { 
                IdPersona = idPersonaLegacy,
                Fecha = DateTime.Now,
                HoraIngreso = DateTime.Now.ToString("HH:mm:ss"), // Varchar compliancy
                PrecioF = items.Sum(i => i.Precio * i.Cantidad),
                IDConvenio = 1, // Rule Enforced by QA
                EstadoDeOrden = 1, // Rule Enforced by QA
                Usuario = int.TryParse(usuarioLocal, out int pId) ? pId : 0 // V11.0 Identificador del sistema nuevo parsing
            };

            var validPerfiles = items
                .Select(i => new 
                { 
                    Precio = i.Precio,
                    PerfilId = serviciosInfo.ContainsKey(i.ServicioId) && 
                              int.TryParse(serviciosInfo[i.ServicioId], out int idLag) ? idLag : 0 
                })
                .Where(p => p.PerfilId > 0)
                .Select(p => new PerfilesFacturadosLegacy { IdPerfil = p.PerfilId, PrecioTotal = p.Precio })
                .ToList();

            if (validPerfiles.Any())
            {
                await _legacyRepository.GenerarOrdenLaboratorioAsync(orden, validPerfiles, new List<ResultadosPacienteLegacy>(), cancellationToken);
            }
        }
    }
}
