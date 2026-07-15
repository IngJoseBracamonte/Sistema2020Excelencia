using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateCuentaAdministrativaCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
        public Guid? NuevoPacienteId { get; set; }
        public string? NuevoTipoIngreso { get; set; }
        public int? NuevoConvenioId { get; set; }
        public List<DetallePrecioCorreccionDto>? CorreccionesPrecios { get; set; }
        public string UsuarioModificacion { get; set; } = "Admin";
    }

    public class DetallePrecioCorreccionDto
    {
        public Guid DetalleId { get; set; }
        public decimal NuevoPrecio { get; set; }
        public decimal NuevoHonorario { get; set; }
        public decimal? NuevaCantidad { get; set; }
    }

    public class UpdateCuentaAdministrativaCommandHandler : IRequestHandler<UpdateCuentaAdministrativaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserAuditLogger _auditLogger;

        public UpdateCuentaAdministrativaCommandHandler(IApplicationDbContext context, IUserAuditLogger auditLogger)
        {
            _context = context;
            _auditLogger = auditLogger;
        }

        public async Task<bool> Handle(UpdateCuentaAdministrativaCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Cargar cuenta de servicios con sus detalles
                var cuenta = await _context.CuentasServicios
                    .Include(c => c.Detalles)
                    .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

                if (cuenta == null)
                {
                    throw new KeyNotFoundException($"No se encontró la cuenta con ID: {request.CuentaId}");
                }

                // --- CAPTURAR ESTADO PREVIO PARA AUDITORÍA ---
                Guid pacienteAnteriorId = cuenta.PacienteId;
                string? pacienteAnteriorNombre = (await _context.PacientesAdmision.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pacienteAnteriorId, cancellationToken))?.NombreCompleto;
                
                string tipoIngresoAnterior = cuenta.TipoIngreso;
                int? convenioAnteriorId = cuenta.ConvenioId;
                string? convenioAnteriorNombre = convenioAnteriorId.HasValue 
                    ? (await _context.SegurosConvenios.AsNoTracking().FirstOrDefaultAsync(c => c.Id == convenioAnteriorId.Value, cancellationToken))?.Nombre 
                    : "PARTICULAR";

                decimal totalAnteriorUSD = cuenta.CalcularTotal();

                var recibo = await _context.RecibosFactura
                    .Include(r => r.DetallesPago)
                    .FirstOrDefaultAsync(r => r.CuentaServicioId == cuenta.Id && r.EstadoFiscal != EstadoConstants.Anulada, cancellationToken);

                decimal reciboTotalAnteriorUSD = recibo?.TotalFacturadoUSD ?? 0;
                decimal reciboVueltoAnteriorUSD = recibo?.MontoVueltoUSD ?? 0;
                decimal reciboPagadoUSD = recibo?.ObtenerTotalPagadoBase() ?? 0;

                var arExistente = await _context.CuentasPorCobrar
                    .FirstOrDefaultAsync(a => a.CuentaServicioId == cuenta.Id, cancellationToken);

                decimal cxcSaldoAnteriorUSD = arExistente?.SaldoPendienteBase ?? 0;

                // Capturar cambios de precios antes de aplicarlos
                var serviceChanges = new List<object>();
                if (request.CorreccionesPrecios != null && request.CorreccionesPrecios.Any())
                {
                    foreach (var corr in request.CorreccionesPrecios)
                    {
                        var detalle = cuenta.Detalles.FirstOrDefault(d => d.Id == corr.DetalleId);
                        if (detalle != null && (detalle.Precio != corr.NuevoPrecio || detalle.Honorario != corr.NuevoHonorario || (corr.NuevaCantidad.HasValue && detalle.Cantidad != corr.NuevaCantidad.Value)))
                        {
                            serviceChanges.Add(new
                            {
                                DetalleId = detalle.Id,
                                Descripcion = detalle.Descripcion,
                                PrecioAnterior = detalle.Precio,
                                PrecioNuevo = corr.NuevoPrecio,
                                HonorarioAnterior = detalle.Honorario,
                                HonorarioNuevo = corr.NuevoHonorario,
                                CantidadAnterior = detalle.Cantidad,
                                CantidadNueva = corr.NuevaCantidad ?? detalle.Cantidad
                            });
                        }
                    }
                }
                string? detalleServiciosCambiosJson = serviceChanges.Any() ? JsonSerializer.Serialize(serviceChanges) : null;

                // 2. Modificar paciente si es provisto
                Guid? pacienteNuevoId = null;
                string? pacienteNuevoNombre = null;

                if (request.NuevoPacienteId.HasValue && request.NuevoPacienteId.Value != cuenta.PacienteId)
                {
                    var pacienteExiste = await _context.PacientesAdmision
                        .AnyAsync(p => p.Id == request.NuevoPacienteId.Value, cancellationToken);

                    if (!pacienteExiste)
                    {
                        throw new InvalidOperationException($"El paciente destino con ID {request.NuevoPacienteId.Value} no existe.");
                    }

                    pacienteNuevoId = request.NuevoPacienteId.Value;
                    pacienteNuevoNombre = (await _context.PacientesAdmision.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pacienteNuevoId.Value, cancellationToken))?.NombreCompleto;

                    // Actualizar en CuentaServicios
                    cuenta.CambiarPacienteAdministrativo(request.NuevoPacienteId.Value);

                    // Actualizar en ReciboFactura
                    var recibos = await _context.RecibosFactura
                        .Where(r => r.CuentaServicioId == cuenta.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var r in recibos)
                    {
                        r.CambiarPacienteAdministrativo(request.NuevoPacienteId.Value);
                    }

                    // Actualizar en CuentaPorCobrar
                    if (arExistente != null)
                    {
                        arExistente.CambiarPacienteAdministrativo(request.NuevoPacienteId.Value);
                    }

                    // Actualizar en CitasMedicas
                    var citas = await _context.CitasMedicas
                        .Where(c => c.CuentaServicioId == cuenta.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var cita in citas)
                    {
                        cita.CambiarPacienteAdministrativo(request.NuevoPacienteId.Value);
                    }

                    // Log de Auditoria
                    await _auditLogger.LogActionAsync(
                        request.UsuarioModificacion,
                        "CAMBIO_PACIENTE",
                        cuenta.Id,
                        $"Paciente cambiado de '{pacienteAnteriorNombre}' (ID: {pacienteAnteriorId}) a '{pacienteNuevoNombre}' (ID: {pacienteNuevoId})."
                    );
                }

                // 3. Modificar tipo de ingreso/convenio si es provisto
                if (!string.IsNullOrEmpty(request.NuevoTipoIngreso))
                {
                    int? convenioId = null;
                    if (request.NuevoTipoIngreso == EstadoConstants.Seguro || 
                        request.NuevoTipoIngreso == EstadoConstants.Hospitalizacion || 
                        request.NuevoTipoIngreso == EstadoConstants.Emergencia)
                    {
                        convenioId = request.NuevoConvenioId;
                    }

                    string? newConvenioName = null;
                    if (convenioId.HasValue)
                    {
                        newConvenioName = (await _context.SegurosConvenios.AsNoTracking().FirstOrDefaultAsync(s => s.Id == convenioId.Value, cancellationToken))?.Nombre;
                    }

                    cuenta.CambiarTipoIngresoAdministrativo(request.NuevoTipoIngreso, convenioId);

                    // Log de Auditoria
                    await _auditLogger.LogActionAsync(
                        request.UsuarioModificacion,
                        "CAMBIO_CONVENIO",
                        cuenta.Id,
                        $"Convenio/Ingreso cambiado de '{tipoIngresoAnterior}' ({convenioAnteriorNombre}) a '{request.NuevoTipoIngreso}' ({newConvenioName ?? "PARTICULAR"})."
                    );
                }

                // 4. Modificar precios de los servicios si son provistos
                if (request.CorreccionesPrecios != null && request.CorreccionesPrecios.Any())
                {
                    foreach (var corr in request.CorreccionesPrecios)
                    {
                        var detalle = cuenta.Detalles.FirstOrDefault(d => d.Id == corr.DetalleId);
                        if (detalle != null)
                        {
                            decimal precioAnterior = detalle.Precio;
                            decimal honorarioAnterior = detalle.Honorario;
                            decimal cantidadAnterior = detalle.Cantidad;

                            bool hasChanged = false;
                            var changeDetails = new List<string>();

                            if (detalle.Precio != corr.NuevoPrecio)
                            {
                                changeDetails.Add($"Precio: {precioAnterior} -> {corr.NuevoPrecio}");
                                hasChanged = true;
                            }
                            if (detalle.Honorario != corr.NuevoHonorario)
                            {
                                changeDetails.Add($"Honorario: {honorarioAnterior} -> {corr.NuevoHonorario}");
                                hasChanged = true;
                            }
                            if (corr.NuevaCantidad.HasValue && detalle.Cantidad != corr.NuevaCantidad.Value)
                            {
                                changeDetails.Add($"Cantidad: {cantidadAnterior} -> {corr.NuevaCantidad.Value}");
                                hasChanged = true;
                            }

                            if (hasChanged)
                            {
                                detalle.ModificarPreciosAdministrativos(corr.NuevoPrecio, corr.NuevoHonorario);
                                if (corr.NuevaCantidad.HasValue)
                                {
                                    detalle.ModificarCantidadAdministrativa(corr.NuevaCantidad.Value);
                                }

                                bool esAnulado = (corr.NuevaCantidad.HasValue && corr.NuevaCantidad.Value == 0) || corr.NuevoPrecio == 0;
                                string accion = esAnulado ? "ANULACION_SERVICIO" : "MODIFICACION_SERVICIO";
                                string descripcionLog = esAnulado
                                    ? $"Servicio '{detalle.Descripcion}' (ID: {detalle.Id}) ANULADO administrativamente. Detalles anteriores: Cantidad={cantidadAnterior}, Precio={precioAnterior}, Honorario={honorarioAnterior}."
                                    : $"Servicio '{detalle.Descripcion}' (ID: {detalle.Id}) modificado. Cambios: {string.Join(", ", changeDetails)}.";

                                await _auditLogger.LogActionAsync(
                                    request.UsuarioModificacion,
                                    accion,
                                    cuenta.Id,
                                    descripcionLog
                                );
                            }
                        }
                    }
                }

                // 5. Recalcular total de la cuenta
                var nuevoTotal = cuenta.CalcularTotal();

                // 6. Recalcular y actualizar ReciboFactura & CuentasPorCobrar
                decimal totalPagado = 0;
                if (recibo != null)
                {
                    totalPagado = recibo.ObtenerTotalPagadoBase();
                    var nuevoMontoVuelto = Math.Max(0, totalPagado - nuevoTotal);
                    recibo.ActualizarTotalesAdministrativos(nuevoTotal, nuevoMontoVuelto);
                }

                CuentaPorCobrar? arNueva = null;
                if (totalPagado < nuevoTotal)
                {
                    if (arExistente == null)
                    {
                        arNueva = new CuentaPorCobrar(cuenta.Id, cuenta.PacienteId, nuevoTotal, totalPagado);
                        arNueva.ActualizarMontoTotalAdministrativo(nuevoTotal);
                        if (cuenta.ConvenioId == null)
                        {
                            arNueva.MarcarComoAuditada(request.UsuarioModificacion);
                        }
                        _context.CuentasPorCobrar.Add(arNueva);
                    }
                    else
                    {
                        arExistente.ActualizarMontoTotalAdministrativo(nuevoTotal);
                    }
                }
                else if (arExistente != null)
                {
                    arExistente.ActualizarMontoTotalAdministrativo(nuevoTotal);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // --- CAPTURAR ESTADO NUEVO Y AGREGAR LOG DE AUDITORÍA ---
                string tipoIngresoNuevo = cuenta.TipoIngreso;
                int? convenioNuevoIdVal = cuenta.ConvenioId;
                string? convenioNuevoNombreVal = "PARTICULAR";

                if (tipoIngresoNuevo != EstadoConstants.Particular && convenioNuevoIdVal.HasValue)
                {
                    convenioNuevoNombreVal = (await _context.SegurosConvenios.AsNoTracking().FirstOrDefaultAsync(c => c.Id == convenioNuevoIdVal.Value, cancellationToken))?.Nombre;
                }

                decimal totalNuevoUSD = cuenta.CalcularTotal();
                decimal reciboTotalNuevoUSD = recibo?.TotalFacturadoUSD ?? 0;
                decimal reciboVueltoNuevoUSD = recibo?.MontoVueltoUSD ?? 0;
                decimal cxcSaldoNuevoUSD = arExistente?.SaldoPendienteBase ?? arNueva?.SaldoPendienteBase ?? 0;

                var logAuditoria = new HistorialModificacionCuenta(
                    cuenta.Id,
                    request.UsuarioModificacion,
                    pacienteAnteriorId,
                    pacienteAnteriorNombre,
                    pacienteNuevoId,
                    pacienteNuevoNombre,
                    tipoIngresoAnterior,
                    tipoIngresoNuevo,
                    convenioAnteriorId,
                    convenioAnteriorNombre,
                    convenioNuevoIdVal,
                    convenioNuevoNombreVal,
                    totalAnteriorUSD,
                    totalNuevoUSD,
                    reciboTotalAnteriorUSD,
                    reciboTotalNuevoUSD,
                    reciboVueltoAnteriorUSD,
                    reciboVueltoNuevoUSD,
                    reciboPagadoUSD,
                    cxcSaldoAnteriorUSD,
                    cxcSaldoNuevoUSD,
                    detalleServiciosCambiosJson
                );

                _context.HistorialModificacionCuentas.Add(logAuditoria);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
