using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SyncCarritoCommand : IRequest<SyncCarritoResult>
    {
        public Guid PacienteId { get; set; }
        public int? IdPacienteLegacy { get; set; } // V11.6: Soporte para Onboarding dinámico
        public string UsuarioCarga { get; set; } = string.Empty;
        public string TipoIngreso { get; set; } = "Particular";
        public int? ConvenioId { get; set; }
        public string? SupervisorKey { get; set; } // V1.0 Security Matrix
        public bool IsPrivilegedUser { get; set; } // V1.0 Security Matrix
        public List<ServicioCarritoDto> Items { get; set; } = new();
    }

    public class ServicioCarritoDto
    {
        public string ServicioId { get; set; } = string.Empty; // V11.9: Soporte String para IDs Legacy (Lab)
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public int Cantidad { get; set; } = 1;
        public string TipoServicio { get; set; } = string.Empty;
        
        // Datos para Citas
        public Guid? MedicoId { get; set; }
        public DateTime? HoraCita { get; set; }
        public string? Comentario { get; set; }
    }

    public record SyncCarritoResult(Guid CuentaId, List<DetalleSyncDto> Detalles);
    public record DetalleSyncDto(Guid ServicioId, Guid DetalleId);

    public class SyncCarritoCommandHandler : IRequestHandler<SyncCarritoCommand, SyncCarritoResult>
    {
        private readonly IBillingRepository _repository;
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly IHonorariumMapperService _mapperService;
        private readonly ILogger<SyncCarritoCommandHandler> _logger;

        public SyncCarritoCommandHandler(
            IBillingRepository repository, 
            IApplicationDbContext context, 
            ILegacyLabRepository legacyRepository,
            IHonorariumMapperService mapperService,
            ILogger<SyncCarritoCommandHandler> logger)
        {
            _repository = repository;
            _context = context;
            _legacyRepository = legacyRepository;
            _mapperService = mapperService;
            _logger = logger;
        }

        public async Task<SyncCarritoResult> Handle(SyncCarritoCommand request, CancellationToken ct)
        {
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("SyncCarrito.Handle");
            activity?.SetTag("paciente.id", request.PacienteId);
            activity?.SetTag("paciente.id_legacy", request.IdPacienteLegacy);
            activity?.SetTag("items.count", request.Items.Count);

            _logger.LogInformation("[SYNC] Iniciando sincronización de carrito (Nuevo Ingreso) - Paciente: {PacienteId}, Legacy: {LegacyId}", request.PacienteId, request.IdPacienteLegacy);
            
            try 
            {
                PacienteAdmision? paciente = null;

                // 1. Resolución de Identidad (V11.6 Onboarding)
                if (request.PacienteId != Guid.Empty)
                {
                    paciente = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                        _context.PacientesAdmision, p => p.Id == request.PacienteId, ct);
                }
                
                // Si no se encontró por GUID, intentar Onboarding por Legacy ID (Concatenación V11.6)
                if (paciente == null && request.IdPacienteLegacy.HasValue)
                {
                    _logger.LogInformation("[SYNC/ONBOARD] Paciente nativo no encontrado. Consultando Legacy ID: {LegacyId}", request.IdPacienteLegacy);
                    var legacyPatient = await _legacyRepository.GetPatientByIdAsync(request.IdPacienteLegacy.Value.ToString(), ct);
                    
                    if (legacyPatient != null)
                    {
                        _logger.LogInformation("[SYNC/ONBOARD] Migrando paciente {Nombre} {Apellidos} desde Legacy...", legacyPatient.Nombre, legacyPatient.Apellidos);
                        var fullName = $"{legacyPatient.Nombre} {legacyPatient.Apellidos}".Trim();
                        var mainPhone = !string.IsNullOrEmpty(legacyPatient.Celular) ? legacyPatient.Celular : legacyPatient.Telefono;
                        
                        paciente = new PacienteAdmision(legacyPatient.Cedula, fullName, mainPhone ?? "", legacyPatient.IdPersona);
                        try 
                        {
                            await _context.PacientesAdmision.AddAsync(paciente, ct);
                            await _context.SaveChangesAsync(ct);
                            _logger.LogInformation("[SYNC/ONBOARD] Identidad Nativa Creada: {NewId}", paciente.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("[SYNC/ONBOARD] Colisión detectada durante creación de paciente. Re-consultando identidad... Detalle: {Msg}", ex.Message);
                            // Limpiar el estado de seguimiento para evitar conflictos de tracking
                            _context.ChangeTracker.Clear();
                            paciente = await _context.PacientesAdmision.FirstOrDefaultAsync(p => p.CedulaPasaporte == legacyPatient.Cedula, ct);
                            
                            if (paciente == null) throw; // Si aún es null, el error es otro
                            _logger.LogInformation("[SYNC/ONBOARD] Paciente recuperado de colisión exitosamente: {Id}", paciente.Id);
                        }
                    }
                }

                if (paciente == null)
                {
                    _logger.LogWarning("[SYNC] Fallo de Identidad: No se encontró registro nativo ni legacy para la solicitud.");
                    throw new InvalidOperationException($"No se pudo resolver la identidad del paciente. Verifique que exista en el sistema Legacy o registro nativo.");
                }

                // 2. CREACIÓN OBLIGATORIA DE CUENTA (Ingreso Nuevo por Wizard - V11.5)
                // Se abandona la búsqueda de cuentas abiertas para garantizar atomicidad por Ingreso.
                _logger.LogInformation("[SYNC] Creando NUEVO INGRESO para el paciente {PacienteId}", paciente.Id);
                var cuenta = new CuentaServicios(paciente.Id, request.UsuarioCarga, request.TipoIngreso, request.ConvenioId);
                await _repository.AgregarCuentaAsync(cuenta, ct);

                // 2b. Validación de Seguridad de Precios en Lote (Fase 1 - Matrix)
                if (!request.IsPrivilegedUser)
                {
                    bool hasPriceModification = false;
                    foreach (var item in request.Items)
                    {
                        if (Guid.TryParse(item.ServicioId, out var svcId))
                        {
                            var baseService = await _context.ServiciosClinicos.AsNoTracking().FirstOrDefaultAsync(s => s.Id == svcId, ct);
                            if (baseService != null)
                            {
                                bool itemEsConsulta = EstadoConstants.EsConsulta(item.TipoServicio) || baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation;
                                decimal expectedPrecio = baseService.PrecioBase;
                                if (itemEsConsulta)
                                {
                                    decimal doctorHonorary = 0;
                                    if (item.MedicoId.HasValue)
                                    {
                                        var medico = await _context.Medicos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == item.MedicoId.Value, ct);
                                        if (medico != null)
                                        {
                                            doctorHonorary = medico.HonorarioBase;
                                        }
                                    }
                                    else
                                    {
                                        doctorHonorary = baseService.HonorarioBase;
                                    }
                                    expectedPrecio = baseService.PrecioBase + doctorHonorary;
                                }

                                if (item.Precio != expectedPrecio && item.Precio != baseService.PrecioBase)
                                {
                                    hasPriceModification = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (hasPriceModification)
                    {
                        var config = await _context.ConfiguracionGeneral.AsNoTracking().FirstOrDefaultAsync(ct);
                        if (config == null || config.ClaveSupervisor != request.SupervisorKey)
                        {
                            throw new InvalidOperationException("La sincronización contiene ediciones de precios y requiere una Clave de Supervisor válida.");
                        }
                        _logger.LogInformation("[SEC] Sincronización con precios modificados autorizada por Clave de Supervisor para {Usuario}", request.UsuarioCarga);
                    }
                }

                _logger.LogDebug("[SYNC] Cuenta de Ingreso establecida: {CuentaId}. Procesando {Count} items.", cuenta.Id, request.Items.Count);

                var detallesRes = new List<DetalleSyncDto>();

                foreach (var item in request.Items)
                {
                    ServicioClinico? baseService = null;
                    Guid serviceGuid = Guid.Empty;
                    if (Guid.TryParse(item.ServicioId, out Guid sGuid))
                    {
                        serviceGuid = sGuid;
                        baseService = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == serviceGuid, ct);
                    }

                    bool esLab = EstadoConstants.EsLaboratorio(item.TipoServicio);
                    bool esConsulta = EstadoConstants.EsConsulta(item.TipoServicio) || (baseService != null && baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation);

                    if (esConsulta)
                    {
                        if (!item.MedicoId.HasValue || !item.HoraCita.HasValue)
                        {
                            _logger.LogWarning("[SYNC] Fallo de Validación: El servicio '{Descripcion}' ({ServicioId}) es de consulta pero no tiene Médico u Horario asignado.", item.Descripcion, item.ServicioId);
                            throw new InvalidOperationException($"El servicio '{item.Descripcion}' requiere la asignación de un médico y un horario de cita para ser facturado.");
                        }

                        // [BUG-FIX] Strict Specialty Validation (V15.0)
                        if (baseService != null)
                        {
                            var medico = await _context.Medicos.FindAsync(new object[] { item.MedicoId.Value }, ct);

                            if (medico != null && baseService.EspecialidadId.HasValue)
                            {
                                if (baseService.EspecialidadId.Value != medico.EspecialidadId)
                                {
                                    _logger.LogError("[SYNC] Mismatch de Especialidad: Servicio '{Serv}' ({SpecS}) vs Médico '{Med}' ({SpecM})", 
                                        baseService.Descripcion, baseService.EspecialidadId, medico.Nombre, medico.EspecialidadId);
                                    throw new InvalidOperationException($"No se puede asignar el médico '{medico.Nombre}' a la consulta de '{baseService.Descripcion}' porque las especialidades no coinciden.");
                                }
                            }
                        }

                        _logger.LogInformation("[SYNC] Registrando cita médica - Medico: {MedicoId}, Hora: {Hora}", item.MedicoId, item.HoraCita);
                        await ProcesarCitaMedicaAsync(item, paciente.Id, cuenta.Id, ct);
                    }

                    // Senior Enrichment V12.2: Capturar LegacyMappingId del catálogo
                    string? legacyId = null;

                    if (esLab)
                    {
                        // Para Laboratorio, el ServicioId ES el ID de Perfil Legacy (ej: "158")
                        legacyId = item.ServicioId;
                        _logger.LogInformation("[SYNC] Lab Item: '{Desc}' → LegacyMappingId='{Id}'", item.Descripcion, legacyId);
                    }
                    else if (baseService != null)
                    {
                        legacyId = baseService.LegacyMappingId;
                    }

                    decimal doctorHonorary = 0;
                    if (esConsulta && baseService != null)
                    {
                        if (item.MedicoId.HasValue)
                        {
                            var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.Id == item.MedicoId.Value, ct);
                            if (medico != null)
                            {
                                doctorHonorary = medico.HonorarioBase;
                            }
                        }
                        else
                        {
                            doctorHonorary = baseService.HonorarioBase;
                        }
                    }

                    decimal finalPrecio = item.Precio;
                    decimal finalHonorario = item.Honorario;
                    if (esConsulta && baseService != null)
                    {
                        if (item.Precio == baseService.PrecioBase && (item.Honorario == 0 || item.Honorario == baseService.HonorarioBase))
                        {
                            finalPrecio = baseService.PrecioBase + doctorHonorary;
                        }

                        if (item.Honorario == 0)
                        {
                            finalHonorario = doctorHonorary;
                        }
                        else if (item.Honorario == doctorHonorary + baseService.PrecioBase)
                        {
                            finalHonorario = doctorHonorary;
                        }
                        else
                        {
                            finalHonorario = item.Honorario;
                        }
                    }

                    var detalle = cuenta.AgregarServicio(
                        serviceGuid, 
                        item.Descripcion, 
                        finalPrecio, 
                        finalHonorario,
                        item.Cantidad, 
                        item.TipoServicio, 
                        request.UsuarioCarga,
                        legacyId);

                    // Auto-asignación de Médico Responsable desde HonorarioConfig (V18.5)
                    if (detalle.Honorario > 0 && !esConsulta)
                    {
                        string? categoriaMapeada = await _mapperService.MapToCategoryAsync(item.TipoServicio, serviceGuid);
                        if (categoriaMapeada != HonorarioConstants.CategoriaOtros)
                        {
                            var config = await _context.HonorariosConfig
                                .FirstOrDefaultAsync(h => h.CategoriaServicio == categoriaMapeada, ct);
                            if (config?.MedicoDefaultId != null)
                            {
                                detalle.AsignarMedicoResponsable(config.MedicoDefaultId.Value, categoriaMapeada);
                                var medicoNombre = (await _context.Medicos.FindAsync(new object[] { config.MedicoDefaultId.Value }, ct))?.Nombre;
                                _context.LogsAsignacionHonorario.Add(new LogAsignacionHonorario(
                                    detalle.Id, item.Descripcion, HonorarioConstants.AccionAsignacionDefault,
                                    null, null, config.MedicoDefaultId.Value, medicoNombre,
                                    request.UsuarioCarga, "Auto-asignado por configuración"));
                            }
                        }
                    }
                    
                    // AUDIT LOG (Phase 9): Detect modification in price OR honorary and log it
                    if (baseService != null)
                    {
                        decimal expectedDefaultPrecio = esConsulta ? (baseService.PrecioBase + doctorHonorary) : baseService.PrecioBase;
                        decimal expectedDefaultHonorario = esConsulta ? doctorHonorary : baseService.HonorarioBase;

                        if (expectedDefaultPrecio != finalPrecio || expectedDefaultHonorario != finalHonorario)
                        {
                            var auditLog = new LogAuditoriaPrecio(
                                detalle.Id,
                                item.Descripcion,
                                expectedDefaultPrecio,
                                finalPrecio,
                                expectedDefaultHonorario,
                                finalHonorario,
                                request.UsuarioCarga,
                                string.IsNullOrEmpty(request.SupervisorKey) ? "Admin Privilegiado" : "Supervisor Key Autorizado"
                            );
                            await _context.AuditLogsPrecios.AddAsync(auditLog, ct);
                            _logger.LogInformation("[AUDIT] Cambio de precio/honorario detectado y registrado para '{Servicio}': P({OldP}->{NewP}), H({OldH}->{NewH})", 
                                item.Descripcion, expectedDefaultPrecio, finalPrecio, expectedDefaultHonorario, finalHonorario);
                        }
                    }

                    detallesRes.Add(new DetalleSyncDto(serviceGuid, detalle.Id));
                }

                _logger.LogInformation("[SYNC] Persistiendo cambios en base de datos para Cuenta: {CuentaId}", cuenta.Id);
                await _repository.GuardarCambiosAsync(ct);

                _logger.LogInformation("[SYNC] Sincronización exitosa. Total Items: {Count}", detallesRes.Count);
                return new SyncCarritoResult(cuenta.Id, detallesRes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SYNC] ERROR CRÍTICO sincronizando carrito para Paciente {PacienteId}. Tipo: {ExceptionType}", request.PacienteId, ex.GetType().Name);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }

        private async Task ProcesarCitaMedicaAsync(ServicioCarritoDto item, Guid pacienteId, Guid cuentaId, CancellationToken ct)
        {
            // Normalización de Horario (V3.1)
            var horaNormalizada = new DateTime(
                item.HoraCita!.Value.Year, item.HoraCita.Value.Month, item.HoraCita.Value.Day,
                item.HoraCita.Value.Hour, item.HoraCita.Value.Minute, 0);

            // Validar disponibilidad
            if (await _repository.ExisteCitaSimultaneaAsync(item.MedicoId!.Value, horaNormalizada, ct))
                throw new InvalidOperationException($"El médico ya tiene una cita ocupada a las {horaNormalizada:HH:mm}.");

            var cita = new CitaMedica(item.MedicoId.Value, pacienteId, cuentaId, horaNormalizada, item.Comentario);
            await _repository.AgregarCitaMedicaAsync(cita, ct);
        }
    }
}
