using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class TrasladarPacienteCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid SedeDestinoId { get; set; }
        public Guid AreaClinicaCamaId { get; set; }
        public string? Observacion { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public decimal? MontoEstanciaUsd { get; set; }
    }

    public class TrasladarPacienteCirugiaCommandHandler : IRequestHandler<TrasladarPacienteCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<TrasladarPacienteCirugiaCommandHandler> _logger;

        public TrasladarPacienteCirugiaCommandHandler(
            IApplicationDbContext context,
            ILogger<TrasladarPacienteCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(TrasladarPacienteCirugiaCommand request, CancellationToken cancellationToken)
        {
            var orden = await _context.OrdenesCirugia
                .Include(o => o.Logs)
                .Include(o => o.HistorialObservaciones)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null)
                throw new InvalidOperationException($"No se encontró la orden de cirugía con ID {request.OrdenCirugiaId}.");

            var sede = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == request.SedeDestinoId, cancellationToken);

            var camaArea = await _context.AreasClinicas
                .FirstOrDefaultAsync(a => a.Id == request.AreaClinicaCamaId, cancellationToken);

            if (camaArea == null)
                throw new InvalidOperationException($"No se encontró la cama, sala o quirófano seleccionado ({request.AreaClinicaCamaId}).");

            var sedeNombre = sede?.Nombre ?? "Área Clínica";
            var camaNombre = camaArea.Nombre;
            var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "Sistema" : request.UsuarioId.Trim();

            // Buscar cuenta de servicios activa si no venía cargada en la navegación
            var cuenta = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.Id == orden.CuentaServicioId || (c.PacienteId == orden.PacienteId && c.Estado == EstadoConstants.Abierta), cancellationToken);

            if (cuenta == null)
            {
                cuenta = new CuentaServicios(
                    orden.PacienteId,
                    usuario,
                    sede?.Nombre?.Contains("Emerg", StringComparison.OrdinalIgnoreCase) == true ? "Emergencia" : "Hospitalizacion",
                    convenioId: null,
                    areaClinicaId: camaArea.Id,
                    subAreaClinica: $"{sedeNombre} - {camaNombre}",
                    medicoId: orden.MedicoId != Guid.Empty ? orden.MedicoId : (Guid?)null
                );
                await _context.CuentasServicios.AddAsync(cuenta, cancellationToken);
            }

            // Determinar si el destino es Quirófano o Sala de Cirugía
            bool esDestinoQuirofano = (sede != null && (sede.Codigo == "CIR" || sede.Nombre.Contains("Cirug", StringComparison.OrdinalIgnoreCase))) ||
                                      camaNombre.Contains("Quiróf", StringComparison.OrdinalIgnoreCase) ||
                                      camaNombre.Contains("Quirof", StringComparison.OrdinalIgnoreCase) ||
                                      camaArea.Codigo.Contains("QX", StringComparison.OrdinalIgnoreCase);

            if (esDestinoQuirofano)
            {
                // ESCENARIO 1: TRASLADO PRE-QUIRÚRGICO (Ingreso a Pabellón / Quirófano)
                // Guardar la ubicación de origen antes de entrar a cirugía para poder retornar al paciente
                if (cuenta.AreaClinicaId.HasValue && cuenta.AreaClinicaId.Value != camaArea.Id)
                {
                    var camaOrigenPrevia = await _context.AreasClinicas
                        .FirstOrDefaultAsync(a => a.Id == cuenta.AreaClinicaId.Value, cancellationToken);

                    if (camaOrigenPrevia != null)
                    {
                        orden.GuardarUbicacionOrigen(camaOrigenPrevia.Id, camaOrigenPrevia.SedeId);
                        // Retener la cama en la cuenta para que no se asigne a otro paciente durante la operación
                        cuenta.AsignarCamaRetenida(camaOrigenPrevia.Id);
                    }
                }

                // Ocupar sala de quirófano
                camaArea.MarcarComoOcupada();

                orden.AsignarSalaYAnestesia(camaNombre, orden.ModalidadAnestesia, usuario);
                if (orden.Estado == EstadoCirugiaConstants.Programada)
                {
                    orden.IniciarEspera(usuario);
                }

                cuenta.AsignarAreaClinica(camaArea.Id, $"{sedeNombre} - {camaNombre}");
            }
            else
            {
                // ESCENARIO 2: TRASLADO POST-QUIRÚRGICO / RETORNO A HABITACIÓN, UCI O EMERGENCIA
                // 1. Si la orden estaba en un quirófano, liberar la sala quirúrgica
                if (orden.AreaClinicaId.HasValue && orden.AreaClinicaId.Value != camaArea.Id)
                {
                    var camaPrevia = await _context.AreasClinicas
                        .FirstOrDefaultAsync(a => a.Id == orden.AreaClinicaId.Value, cancellationToken);
                    if (camaPrevia != null)
                    {
                        camaPrevia.Liberar();
                    }
                }

                // 2. Si el paciente tenía una cama previa retenida y el nuevo destino es DISTINTO (ej. va a UCI en vez de volver a su habitación de origen), liberar la cama retenida
                if (cuenta.CamaRetenidaId.HasValue && cuenta.CamaRetenidaId.Value != camaArea.Id)
                {
                    var camaRetenidaPrevia = await _context.AreasClinicas
                        .FirstOrDefaultAsync(a => a.Id == cuenta.CamaRetenidaId.Value, cancellationToken);
                    if (camaRetenidaPrevia != null)
                    {
                        camaRetenidaPrevia.Liberar();
                    }
                }

                // 3. Ocupar la nueva cama destino (ej. Cama UCI o Habitación de Hospitalización)
                camaArea.MarcarComoOcupada();

                // 4. Actualizar cuenta con la nueva ubicación
                cuenta.AsignarAreaClinica(camaArea.Id, $"{sedeNombre} - {camaNombre}");
                cuenta.AsignarCamaRetenida(camaArea.Id);

                // 5. Vincular cargo de tarifa de estancia si aplica
                if (request.MontoEstanciaUsd.HasValue && request.MontoEstanciaUsd.Value > 0)
                {
                    ServicioClinico? servicioCatalogo = null;
                    if (camaArea.ServicioTarifaBaseId.HasValue)
                    {
                        servicioCatalogo = await _context.ServiciosClinicos
                            .FirstOrDefaultAsync(s => s.Id == camaArea.ServicioTarifaBaseId.Value, cancellationToken);
                    }

                    if (servicioCatalogo == null)
                    {
                        string areaUpper = sedeNombre.ToUpperInvariant();
                        string codigoServicio = areaUpper.Contains("UCI") || areaUpper.Contains("INTENSIV") ? "HOSP-UCI-01" :
                                                areaUpper.Contains("EMERG") ? "HOSP-EMG-01" : "HOSP-HOS-01";

                        servicioCatalogo = await _context.ServiciosClinicos
                            .FirstOrDefaultAsync(s => s.Codigo == codigoServicio, cancellationToken)
                            ?? await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.TipoServicio == "Hospitalario", cancellationToken);
                    }

                    if (servicioCatalogo != null)
                    {
                        var nuevoDetalle = cuenta.AgregarServicio(
                            servicioCatalogo.Id,
                            $"{servicioCatalogo.Descripcion} ({sedeNombre} - {camaNombre}) - Post-Quirúrgico",
                            request.MontoEstanciaUsd.Value,
                            0,
                            1,
                            servicioCatalogo.TipoServicio ?? "Hospitalario",
                            usuario,
                            servicioCatalogo.LegacyMappingId,
                            camaArea.Id
                        );
                        await _context.DetallesServicioCuenta.AddAsync(nuevoDetalle, cancellationToken);
                    }
                }
            }

            var obsTexto = string.IsNullOrWhiteSpace(request.Observacion) ? "" : $" | Obs: {request.Observacion.Trim()}";
            
            // Registrar auditoría inmutable a través de la raíz agregada
            var log = orden.AgregarLog(usuario, "Traslado", $"Paciente trasladado a {sedeNombre} - {camaNombre}{obsTexto}");
            var hist = orden.AgregarHistorialObservacion($"Traslado a {sedeNombre} - {camaNombre}{obsTexto}", TipoObservacionCirugia.ObservacionMedica, usuario, usuario);

            await _context.CirugiaLogs.AddAsync(log, cancellationToken);
            await _context.CirugiasObservacionesHistorial.AddAsync(hist, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Paciente trasladado exitosamente a {Sede} - {Cama} en orden {OrdenId}", sedeNombre, camaNombre, orden.Id);
            return true;
        }
    }
}
