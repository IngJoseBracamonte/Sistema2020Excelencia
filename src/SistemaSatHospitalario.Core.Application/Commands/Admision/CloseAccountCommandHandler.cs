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
    public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, CloseAccountResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly ICajaAdministrativaRepository _cajaRepository;
        private readonly IBillingRepository _billingRepository;
        private readonly ILegacyErrorReportingService _logger;
        private readonly IOrdenExternaService _ordenExternaService;

        public CloseAccountCommandHandler(
            IApplicationDbContext context, 
            ILegacyLabRepository legacyRepository,
            ICajaAdministrativaRepository cajaRepository,
            IBillingRepository billingRepository,
            ILegacyErrorReportingService logger,
            IOrdenExternaService ordenExternaService)
        {
            _context = context;
            _legacyRepository = legacyRepository;
            _cajaRepository = cajaRepository;
            _billingRepository = billingRepository;
            _logger = logger;
            _ordenExternaService = ordenExternaService;
        }

        public async Task<CloseAccountResult> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogTrace($"[CLOSE-ACCOUNT] Iniciando proceso para Cuenta: {request.CuentaId}");

            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            if (cuenta == null) throw new Exception("Cuenta no encontrada.");
            if (cuenta.Estado != EstadoConstants.Abierta) throw new Exception("La cuenta ya ha sido procesada.");

            // Resolver consolidación si aplica
            List<CuentaServicios> accountsToBill;
            if (request.Consolidar)
            {
                var rootId = cuenta.CuentaPrincipalId ?? cuenta.Id;
                var allChainAccounts = await _context.CuentasServicios
                    .Include(c => c.Detalles)
                    .Where(c => c.Id == rootId || c.CuentaPrincipalId == rootId)
                    .ToListAsync(cancellationToken);

                var billedAccountIds = await _context.RecibosFactura
                    .Where(rf => rf.CuentaServicioId != request.CuentaId)
                    .Select(rf => rf.CuentaServicioId)
                    .ToListAsync(cancellationToken);

                var arAccountIds = await _context.CuentasPorCobrar
                    .Where(ar => ar.CuentaServicioId != request.CuentaId)
                    .Select(ar => ar.CuentaServicioId)
                    .ToListAsync(cancellationToken);

                accountsToBill = allChainAccounts
                    .Where(c => !billedAccountIds.Contains(c.Id) && !arAccountIds.Contains(c.Id))
                    .ToList();
            }
            else
            {
                accountsToBill = new List<CuentaServicios> { cuenta };
            }

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
            var metodosPagoCatalog = await _context.CatalogoMetodosPago
                .Where(m => m.Activo)
                .ToListAsync(cancellationToken);

            decimal totalCuenta = accountsToBill.Sum(c => c.CalcularTotal());
            decimal totalPagado = 0;

            var listaPagosValidados = new List<(DetallePagoDto Pago, decimal Equivalente, decimal Tasa)>();

            foreach (var p in request.Pagos)
            {
                var metodoPagoEntidad = metodosPagoCatalog.FirstOrDefault(m => m.Valor == p.MetodoPago || m.Nombre == p.MetodoPago);
                decimal equivalenteBase = p.EquivalenteAbonadoBase;
                decimal tasaAplicada = 1m;

                if (metodoPagoEntidad != null)
                {
                    if (metodoPagoEntidad.GrupoMoneda == 1)
                    {
                        tasaAplicada = 1m;
                        equivalenteBase = p.MontoAbonadoMoneda;
                    }
                    else if (metodoPagoEntidad.GrupoMoneda == 2)
                    {
                        tasaAplicada = request.TasaCambio;
                        if (tasaAplicada <= 0) throw new InvalidOperationException("La tasa de cambio debe ser mayor a cero para pagos en Bolívares.");
                        equivalenteBase = Math.Round(p.MontoAbonadoMoneda / tasaAplicada, 2);
                    }
                }
                else
                {
                    // Fallback
                    var lower = p.MetodoPago.ToLower();
                    if (lower.Contains("bs") || lower.Contains("móvil") || lower.Contains("punto"))
                    {
                        tasaAplicada = request.TasaCambio;
                        equivalenteBase = tasaAplicada > 0 ? Math.Round(p.MontoAbonadoMoneda / tasaAplicada, 2) : 0;
                    }
                    else
                    {
                        tasaAplicada = 1m;
                        equivalenteBase = p.MontoAbonadoMoneda;
                    }
                }

                totalPagado += equivalenteBase;
                listaPagosValidados.Add((p, equivalenteBase, tasaAplicada));
            }

            decimal montoVueltoUSD = Math.Max(0, totalPagado - totalCuenta);

            var numeroComprobante = await GenerarSiguienteNumeroComprobanteAsync(cancellationToken);
            var recibo = new ReciboFactura(cuenta.Id, cuenta.PacienteId, caja.Id, request.TasaCambio, totalCuenta, montoVueltoUSD, EstadoConstants.Borrador, numeroComprobante);
            foreach (var item in listaPagosValidados)
            {
                recibo.AgregarDetallePago(item.Pago.MetodoPago, item.Pago.ReferenciaBancaria, item.Pago.MontoAbonadoMoneda, item.Equivalente, item.Tasa, request.UsuarioCajero);
            }

            // 2. Gestionar Saldo Pendiente (AR)
            // decimal totalCuenta = cuenta.CalcularTotal(); // Ya calculado arriba
            // decimal totalPagado = recibo.ObtenerTotalPagadoBase(); // Ya calculado arriba

            // 4. Finalizar Cuenta (V10.9 SQL Direct Fix)
            // Usamos SQL Directo para puentear el Change Tracker de EF y evitar el error "Affected 0 rows"
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            CuentaPorCobrar? ar = null;
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
                    ar = new CuentaPorCobrar(cuenta.Id, cuenta.PacienteId, totalCuenta, totalPagado);
                    if (cuenta.ConvenioId == null)
                    {
                        ar.MarcarComoAuditada("Sistema");
                    }
                    _context.CuentasPorCobrar.Add(ar);
                }

                // Extraemos items para pasarlos después
                var itemsLab = accountsToBill.SelectMany(c => c.Detalles).Where(d => EstadoConstants.EsLaboratorio(d.TipoServicio)).ToList();

                // 4.3 EJECUCIÓN NUCLEAR: Actualización Directa en DB vía Repositorio
                // Previene DbUpdateConcurrencyException (V10.9 SQL Direct)
                foreach (var acc in accountsToBill)
                {
                    acc.Facturar();
                    if (acc.Id == request.CuentaId)
                    {
                        acc.RegistrarDestinoEgreso(request.DestinoPaciente, request.PersonalRelevo);
                    }
                    string? dest = acc.Id == request.CuentaId ? request.DestinoPaciente : null;
                    string? relevo = acc.Id == request.CuentaId ? request.PersonalRelevo : null;
                    await _billingRepository.ForzarCierreCuentaAsync(acc.Id, DateTime.UtcNow, dest, relevo, cancellationToken);
                }

                // Si es un traslado clínico interno, creamos automáticamente una nueva cuenta (hija) en la ubicación destino
                bool esTrasladoClinico = request.DestinoPaciente == "Hospitalización (Piso)" || 
                                         request.DestinoPaciente == "Quirófano" || 
                                         request.DestinoPaciente == "UCI";

                if (esTrasladoClinico)
                {
                    Guid parentCuentaId = cuenta.CuentaPrincipalId ?? cuenta.Id;
                    var nuevaCuenta = new CuentaServicios(
                        cuenta.PacienteId,
                        request.UsuarioCajero ?? "cajero",
                        EstadoConstants.Hospitalizacion, // "Hospitalizacion"
                        cuenta.ConvenioId
                    );
                    
                    nuevaCuenta.VincularCuentaPrincipal(parentCuentaId);
                    await _context.CuentasServicios.AddAsync(nuevaCuenta, cancellationToken);
                }

                // 4.4 Persistimos los demás objetos locales (Recibo, AR)
                _context.RecibosFactura.Add(recibo);
                await _context.SaveChangesAsync(cancellationToken);
                
                // 4.5 EJECUCIÓN LEGACY: Justo antes del Commit local para prevenir órdenes huérfanas en MySQL si lo local revienta
                if (itemsLab.Any())
                {
                    _logger.LogTrace($"[CLOSE-ACCOUNT] Procesando {itemsLab.Count} ítems de Laboratorio para Legado.");
                    var legacyOrderId = await ProcessLegacyOrder(cuenta, itemsLab, cancellationToken);
                    if (legacyOrderId > 0)
                    {
                        cuenta.AsignarLegacyOrder(legacyOrderId);
                        await _billingRepository.ActualizarCuentaAsync(cuenta, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }

                // 4.6 EJECUCIÓN IMÁGENES (RX/TOMO) - Fase 16.2
                var itemsImaging = accountsToBill.SelectMany(c => c.Detalles).ToList();
                await ProcessImagingOrders(cuenta.PacienteId, itemsImaging, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                _logger.LogTrace($"[CLOSE-ACCOUNT] Cuenta cerrada exitosamente: {cuenta.Id}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError($"[CLOSE-ACCOUNT] Error Crítico en Cuenta {request.CuentaId}", ex);
                
                string errorMsg = ex.InnerException is InvalidOperationException 
                    ? $"Error de Infraestructura: {ex.Message}" 
                    : $"Error crítico en el cierre de cuenta (V10.9): {ex.Message}";
                    
                throw new Exception(errorMsg, ex);
            }

            return new CloseAccountResult
            {
                ReciboId = recibo.Id,
                CuentaId = cuenta.Id,
                CuentaPorCobrarId = ar?.Id,
                TotalUsd = recibo.TotalFacturadoUSD,
                SincronizacionLegacyExitosa = true, // If we reached here, commit was successful
                Mensaje = "Cuenta cerrada y sincronizada exitosamente."
            };
        }

        private async Task<int> ProcessLegacyOrder(CuentaServicios cuenta, List<DetalleServicioCuenta> labItems, CancellationToken ct)
        {
            _logger.LogTrace($"[LEGACY-SYNC] === INICIO ProcessLegacyOrder para CuentaId: {cuenta.Id} ===");
            
            // Senior Logic: Sincronización basado en IDs persistentes (V12.1 ID-First)
            var paciente = await _context.PacientesAdmision
                .FirstOrDefaultAsync(p => p.Id == cuenta.PacienteId, ct);

            if (paciente == null) 
            {
                _logger.LogTrace($"[LEGACY-SYNC] ABORTADO: No se encontró el paciente nativo {cuenta.PacienteId}");
                return 0;
            }
            
            _logger.LogTrace($"[LEGACY-SYNC] Paciente encontrado: {paciente.CedulaPasaporte} | NombreCorto: {paciente.NombreCorto} | IdLegacy: {paciente.IdPacienteLegacy}");

            // V12.3 JIT Sync Logic (Rule 7)
            if (!paciente.IdPacienteLegacy.HasValue || paciente.IdPacienteLegacy.Value == 0)
            {
                _logger.LogTrace($"[LEGACY-SYNC] JIT: El paciente {paciente.CedulaPasaporte} {paciente.NombreCorto} no tiene ID Legacy. Intentando Onboarding...");
                
                var existinLegacy = await _legacyRepository.GetPatientByCedulaAsync(paciente.CedulaPasaporte, ct);
                if (existinLegacy != null)
                {
                    paciente.VincularLegacy(existinLegacy.IdPersona);
                    _logger.LogTrace($"[LEGACY-SYNC] JIT: Paciente encontrado en Legacy (ID: {existinLegacy.IdPersona}). Vinculando...");
                }
                else
                {
                    _logger.LogTrace($"[LEGACY-SYNC] JIT: No existe en Legacy. Creando nuevo registro...");
                    var legacyPatient = new DatosPersonalesLegacy
                    {
                        Cedula = paciente.CedulaPasaporte,
                        Nombre = paciente.NombreCorto,
                        Apellidos = "",
                        Sexo = "M",
                        Fecha = DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd"),
                        Celular = paciente.TelefonoContact ?? "",
                        Telefono = "",
                        Correo = "",
                        TipoCorreo = "@gmail.com",
                        CodigoCelular = "0414",
                        CodigoTelefono = "0212"
                    };
                    int newId = await _legacyRepository.CreatePatientLegacyAsync(legacyPatient, ct);
                    if (newId > 0)
                    {
                        paciente.VincularLegacy(newId);
                        _logger.LogTrace($"[LEGACY-SYNC] JIT: Paciente creado exitosamente en Legacy (ID: {newId})");
                    }
                    else
                    {
                        _logger.LogTrace($"[LEGACY-SYNC] ERROR: Falló la creación del paciente en Legacy.");
                        return 0;
                    }
                }
                
                await _context.SaveChangesAsync(ct);
            }

            int legacyId = paciente.IdPacienteLegacy!.Value;
            _logger.LogTrace($"[LEGACY-SYNC] LegacyId resuelto: {legacyId}");

            _logger.LogTrace($"[LEGACY-SYNC] Total detalles en cuenta: {labItems.Count}");
            foreach (var d in labItems)
            {
                _logger.LogTrace($"[LEGACY-SYNC]   -> Detalle: '{d.Descripcion}' | TipoServicio: '{d.TipoServicio}' | EsLab: {EstadoConstants.EsLaboratorio(d.TipoServicio)} | LegacyMappingId: '{d.LegacyMappingId}'");
            }

            if (!labItems.Any())
            {
                _logger.LogTrace($"[LEGACY-SYNC] ABORTADO: No hay ítems de laboratorio después del filtro EsLaboratorio");
                return 0;
            }

            var perfilesFacturados = new List<PerfilesFacturadosLegacy>();
            foreach (var item in labItems)
            {
                string? mappingId = item.LegacyMappingId;
                _logger.LogTrace($"[LEGACY-SYNC] Item Lab: '{item.Descripcion}' | MappingId raw: '{mappingId}'");

                if (string.IsNullOrEmpty(mappingId) && item.Descripcion.Contains(EstadoConstants.PrefixLab))
                {
                    // Senior Self-Healing: Extract first positive integer after the prefix
                    var match = Regex.Match(item.Descripcion, $@"{Regex.Escape(EstadoConstants.PrefixLab)}(\d+)");
                    if (match.Success)
                    {
                        mappingId = match.Groups[1].Value;
                        _logger.LogTrace($"[LEGACY-SYNC] Self-Healing: extraído mappingId='{mappingId}' vía Regex desde descripción");
                    }
                }

                if (!string.IsNullOrEmpty(mappingId) && int.TryParse(mappingId, out int idPerfil))
                {
                    perfilesFacturados.Add(new PerfilesFacturadosLegacy 
                    { 
                        IdOrden = 0,
                        IdPersona = legacyId,
                        IdPerfil = idPerfil, 
                        PrecioPerfil = item.Precio * item.Cantidad
                    });
                    _logger.LogTrace($"[LEGACY-SYNC] Perfil agregado: IdPerfil={idPerfil}, Precio={item.Precio * item.Cantidad}");
                }
                else
                {
                    _logger.LogTrace($"[LEGACY-SYNC] DESCARTADO: mappingId='{mappingId}' no es un int válido");
                }
            }

            if (!perfilesFacturados.Any())
            {
                _logger.LogTrace($"[LEGACY-SYNC] ABORTADO: No se generaron perfiles facturados válidos. La orden NO se creará.");
                return 0;
            }

            _logger.LogTrace($"[LEGACY-SYNC] Generando orden con {perfilesFacturados.Count} perfiles para paciente legacy {legacyId}...");
            
            var orden = new OrdenLegacy
            {
                IdPersona = legacyId,
                IDConvenio = cuenta.ConvenioId ?? 1,
                Fecha = DateTime.Now,
                HoraIngreso = DateTime.Now.ToString("HH:mm:ss"),
                PrecioF = perfilesFacturados.Sum(p => p.PrecioPerfil)
            };

            var idOrden = await _legacyRepository.GenerarOrdenLaboratorioAsync(orden, perfilesFacturados, new List<ResultadosPacienteLegacy>(), ct);
            _logger.LogTrace($"[LEGACY-SYNC] === FIN ProcessLegacyOrder - Orden generada exitosamente (ID: {idOrden}) ===");
            return idOrden;
        }

        private async Task ProcessImagingOrders(Guid patientId, List<DetalleServicioCuenta> detalles, CancellationToken ct)
        {
            var items = detalles.Where(d => d.TipoServicio == EstadoConstants.RX || d.TipoServicio == EstadoConstants.TOMO).ToList();
            if (!items.Any()) return;

            var paciente = await _context.PacientesAdmision.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patientId, ct);
            string nombrePaciente = paciente?.NombreCompleto ?? "Paciente Desconocido";

            foreach (var item in items)
            {
                if (item.TipoServicio == EstadoConstants.RX)
                {
                    await _ordenExternaService.EnviarOrdenRXAsync(item.CuentaServicioId, patientId, item.Descripcion, nombrePaciente, ct);
                }
                else if (item.TipoServicio == EstadoConstants.TOMO)
                {
                    await _ordenExternaService.EnviarOrdenTomoAsync(item.CuentaServicioId, patientId, item.Descripcion, nombrePaciente, ct);
                }
            }
        }

        private async Task<string> GenerarSiguienteNumeroComprobanteAsync(CancellationToken cancellationToken)
        {
            var prefix = DateTime.Now.ToString("ddMMyy");
            var count = await _context.RecibosFactura
                .Where(r => r.NumeroComprobante != null && r.NumeroComprobante.StartsWith(prefix + "-"))
                .CountAsync(cancellationToken);
            return $"{prefix}-{(count + 1):D2}";
        }
    }
}
