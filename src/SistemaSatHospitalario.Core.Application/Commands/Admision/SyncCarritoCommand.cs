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

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SyncCarritoCommand : IRequest<SyncCarritoResult>
    {
        public Guid PacienteId { get; set; }
        public int? IdPacienteLegacy { get; set; } // V11.6: Soporte para Onboarding dinámico
        public string UsuarioCarga { get; set; } = string.Empty;
        public string TipoIngreso { get; set; } = "Particular";
        public int? ConvenioId { get; set; }
        public List<ServicioCarritoDto> Items { get; set; } = new();
    }

    public class ServicioCarritoDto
    {
        public Guid ServicioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
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
        private readonly ILogger<SyncCarritoCommandHandler> _logger;

        public SyncCarritoCommandHandler(
            IBillingRepository repository, 
            IApplicationDbContext context, 
            ILegacyLabRepository legacyRepository,
            ILogger<SyncCarritoCommandHandler> logger)
        {
            _repository = repository;
            _context = context;
            _legacyRepository = legacyRepository;
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

                _logger.LogDebug("[SYNC] Cuenta de Ingreso establecida: {CuentaId}. Procesando {Count} items.", cuenta.Id, request.Items.Count);

                var detallesRes = new List<DetalleSyncDto>();

                foreach (var item in request.Items)
                {
                    if (EstadoConstants.EsConsulta(item.TipoServicio) && item.MedicoId.HasValue && item.HoraCita.HasValue)
                    {
                        _logger.LogInformation("[SYNC] Registrando cita médica - Medico: {MedicoId}, Hora: {Hora}", item.MedicoId, item.HoraCita);
                        await ProcesarCitaMedicaAsync(item, paciente.Id, cuenta.Id, ct);
                    }

                    var detalle = cuenta.AgregarServicio(
                        item.ServicioId, 
                        item.Descripcion, 
                        item.Precio, 
                        item.Cantidad, 
                        item.TipoServicio, 
                        request.UsuarioCarga);
                    
                    detallesRes.Add(new DetalleSyncDto(item.ServicioId, detalle.Id));
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
