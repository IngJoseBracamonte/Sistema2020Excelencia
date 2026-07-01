using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CargarServicioACuentaCommand : IRequest<CargarServicioResult>, IAuditablePriceRequest
    {
        // Se estandarizó de int a Guid para identidad nativa (V11.1)
        public Guid PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty; // Particular, Seguro, Hospitalizacion, Emergencia
        // Se mantiene int? para ConvenioId por ahora (referencia Legacy)
        public int? ConvenioId { get; set; }
        public string ServicioId { get; set; } = string.Empty; // V11.9 Support
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoServicio { get; set; } = string.Empty; // Medico, RX, Laboratorio, Insumo
        public string UsuarioCarga { get; set; } = string.Empty;
        public string? SupervisorKey { get; set; } // V1.0 Security Matrix
        public bool IsPrivilegedUser { get; set; } // V1.0 Security Matrix
        public decimal? PrecioModificado { get; set; }
        public decimal? HonorarioModificado { get; set; }

        // Datos para Cita Médica (solo si TipoServicio == "Medico")
        public Guid? MedicoId { get; set; }
        public DateTime? HoraCita { get; set; }

        // Datos para Cirugía/Procedimiento Complejo con Múltiples Médicos
        public global::System.Collections.Generic.List<MedicoRolInputDto>? MedicosRoles { get; set; }
    }

    public class MedicoRolInputDto
    {
        public Guid MedicoId { get; set; }
        public string Rol { get; set; } = string.Empty;
        public decimal MontoHonorario { get; set; }
    }

    public record CargarServicioResult(Guid CuentaId, Guid DetalleId);

    public class CargarServicioACuentaCommandHandler : IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult>
    {
        private readonly IBillingRepository _repository;
        private readonly IOrdenExternaService _externaService;
        private readonly IApplicationDbContext _context;
        private readonly IHonorariumMapperService _mapperService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<CargarServicioACuentaCommandHandler> _logger;

        public CargarServicioACuentaCommandHandler(
            IBillingRepository repository, 
            IOrdenExternaService externaService, 
            IApplicationDbContext context, 
            IHonorariumMapperService mapperService, 
            IInventoryService inventoryService,
            ILogger<CargarServicioACuentaCommandHandler> logger)
        {
            _repository = repository;
            _externaService = externaService;
            _context = context;
            _mapperService = mapperService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<CargarServicioResult> Handle(CargarServicioACuentaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando CargarServicioACuenta para Paciente {PacienteId}, Servicio {ServicioId}", request.PacienteId, request.ServicioId);

            // 1. Obtención Directa del Paciente (V11.1 Identity Alignment)
            var paciente = await _context.PacientesAdmision.FirstOrDefaultAsync(
                p => p.Id == request.PacienteId, cancellationToken);

            if (paciente == null)
            {
                throw new InvalidOperationException($"No se encontró un paciente con el ID: {request.PacienteId}. Asegúrese de registrarlo primero.");
            }

            // 2. Validación de Seguridad de Precios (Fase 1 - Matrix)
            ServicioClinico? baseService = null;
            if (Guid.TryParse(request.ServicioId, out var svcId))
            {
                baseService = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == svcId, cancellationToken);
            }

            bool esConsulta = EstadoConstants.EsConsulta(request.TipoServicio) || (baseService != null && baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation);

            await ValidarPrecioYClaveSupervisorAsync(request, baseService, esConsulta, cancellationToken);

            // 3. Asegurar cuenta activa usando el GUID local
            var cuenta = await GetOrCreateCuentaAsync(paciente.Id, request, cancellationToken);

            // 3. Procesar lógica específica de Consultas/Citas
            if (esConsulta)
            {
                await ProcesarCitaMedicaAsync(request, paciente.Id, cuenta.Id, cancellationToken);
            }

            // Senior Enrichment: Capturar LegacyMappingId del catálogo (V12.2)
            string? legacyId = null;
            bool esLab = EstadoConstants.EsLaboratorio(request.TipoServicio);

            if (esLab)
            {
                // Para Laboratorio, el ServicioId ES el ID de Perfil (Mapeo Legado)
                legacyId = request.ServicioId;
                if (string.IsNullOrEmpty(legacyId)) throw new InvalidOperationException("Se requiere un ID de perfil para servicios de Laboratorio.");
            }
            else if (baseService != null)
            {
                // Para servicios nativos, buscamos en el catálogo el mapeo si existe
                legacyId = baseService.LegacyMappingId;
            }

            decimal finalPrecio = request.PrecioModificado ?? request.Precio;
            decimal finalHonorario = request.HonorarioModificado ?? request.Honorario;

            if (!request.PrecioModificado.HasValue && !request.HonorarioModificado.HasValue)
            {
                if (esConsulta && baseService != null)
                {
                    decimal doctorHonorary = 0;
                    if (request.MedicoId.HasValue)
                    {
                        var medico = await _context.Medicos.FirstOrDefaultAsync(m => m.Id == request.MedicoId.Value, cancellationToken);
                        if (medico != null)
                        {
                            doctorHonorary = medico.HonorarioBase;
                        }
                    }
                    else
                    {
                        doctorHonorary = baseService.HonorarioBase;
                    }

                    if (request.Precio == baseService.PrecioBase && (request.Honorario == 0 || request.Honorario == baseService.HonorarioBase))
                    {
                        finalPrecio = baseService.PrecioBase + doctorHonorary;
                    }

                    if (request.Honorario == 0)
                    {
                        finalHonorario = doctorHonorary;
                    }
                    else if (request.Honorario == doctorHonorary + baseService.PrecioBase)
                    {
                        finalHonorario = doctorHonorary;
                    }
                    else
                    {
                        finalHonorario = request.Honorario;
                    }
                }
            }

            var detalle = cuenta.AgregarServicio(
                esLab ? Guid.Empty : (Guid.TryParse(request.ServicioId, out var g) ? g : Guid.Empty), 
                request.Descripcion, 
                finalPrecio, 
                finalHonorario,
                request.Cantidad, 
                request.TipoServicio, 
                request.UsuarioCarga,
                legacyId);

            if (_context.DetallesServicioCuenta != null)
            {
                _context.DetallesServicioCuenta.Add(detalle);
            }

            // Auto-asignación de Médico Responsable y honorarios
            await AsignarMedicosYHonorariosAsync(request, detalle, esConsulta, cancellationToken);

            // 5. Notificaciones e Integraciones Externas
            await NotificarSistemasExternosAsync(request, cancellationToken);

            // Deduct stock for inventory items associated with this service
            await _inventoryService.DeductInventoryForServiceDetailAsync(
                detalle.Id,
                detalle.ServicioId,
                baseService?.Codigo ?? string.Empty,
                detalle.Descripcion,
                detalle.Cantidad,
                request.UsuarioCarga,
                cuenta.Id,
                null,
                cancellationToken
            );

            await _repository.GuardarCambiosAsync(cancellationToken);

            _logger.LogInformation("Servicio cargado exitosamente en cuenta {CuentaId}. Detalle: {DetalleId}", cuenta.Id, detalle.Id);

            return new CargarServicioResult(cuenta.Id, detalle.Id);
        }

        private async Task ValidarPrecioYClaveSupervisorAsync(
            CargarServicioACuentaCommand request,
            ServicioClinico? baseService,
            bool esConsulta,
            CancellationToken cancellationToken)
        {
            if (request.IsPrivilegedUser || baseService == null) return;

            decimal expectedPrecio = baseService.PrecioBase;
            if (esConsulta)
            {
                decimal doctorHonorary = 0;
                if (request.MedicoId.HasValue)
                {
                    var medico = await _context.Medicos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.MedicoId.Value, cancellationToken);
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

            if (request.Precio != expectedPrecio && request.Precio != baseService.PrecioBase)
            {
                // El precio ha sido modificado, requiere Clave de Supervisor
                var config = await _context.ConfiguracionGeneral.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (config == null || config.ClaveSupervisor != request.SupervisorKey)
                {
                    _logger.LogWarning("[SEC-WARN] Intento de modificación de precio no autorizado por {Usuario}. Esperado: {Orig}, Enviado: {New}",
                        request.UsuarioCarga, expectedPrecio, request.Precio);
                    throw new InvalidOperationException("La modificación de precios requiere una Clave de Supervisor válida.");
                }
                _logger.LogInformation("[SEC] Precio modificado por {Usuario} con Clave de Supervisor válida. Original: {Orig}, Nuevo: {New}", 
                    request.UsuarioCarga, expectedPrecio, request.Precio);
            }
        }

        private async Task AsignarMedicosYHonorariosAsync(
            CargarServicioACuentaCommand request,
            DetalleServicioCuenta detalle,
            bool esConsulta,
            CancellationToken cancellationToken)
        {
            if (request.MedicosRoles != null && request.MedicosRoles.Any())
            {
                decimal totalHonorarios = 0;
                foreach (var mr in request.MedicosRoles)
                {
                    detalle.AgregarMedicoResponsable(mr.MedicoId, mr.Rol, mr.MontoHonorario);
                    totalHonorarios += mr.MontoHonorario;

                    var medicoNombre = (await _context.Medicos.FindAsync(new object[] { mr.MedicoId }, cancellationToken))?.Nombre;
                    _context.LogsAsignacionHonorario.Add(new LogAsignacionHonorario(
                        detalle.Id, request.Descripcion, HonorarioConstants.AccionAsignacionManual,
                        null, null, mr.MedicoId, medicoNombre,
                        request.UsuarioCarga, $"Asignado rol {mr.Rol} en cirugía compleja"));
                }
                
                // Actualizar el honorario acumulado del detalle del servicio
                detalle.ModificarPreciosAdministrativos(detalle.Precio, totalHonorarios);
                _logger.LogInformation("Asignados múltiples médicos para cirugía compleja en detalle {DetalleId}. Total Honorarios: {Total}", detalle.Id, totalHonorarios);
            }
            else if (detalle.Honorario > 0 && !esConsulta)
            {
                Guid? serviceId = Guid.TryParse(request.ServicioId, out var sid) ? sid : null;
                string? categoriaMapeada = await _mapperService.MapToCategoryAsync(request.TipoServicio, serviceId);
                Guid? finalMedicoId = request.MedicoId;
                string sourceAccion = HonorarioConstants.AccionAsignacionManual;

                if ((!finalMedicoId.HasValue || finalMedicoId.Value == Guid.Empty) && categoriaMapeada != HonorarioConstants.CategoriaOtros)
                {
                    var config = await _context.HonorariosConfig
                        .FirstOrDefaultAsync(h => h.CategoriaServicio == categoriaMapeada, cancellationToken);
                    if (config?.MedicoDefaultId != null)
                    {
                        finalMedicoId = config.MedicoDefaultId;
                        sourceAccion = HonorarioConstants.AccionAsignacionDefault;
                    }
                }

                if (finalMedicoId.HasValue && finalMedicoId.Value != Guid.Empty && serviceId.HasValue)
                {
                    // Buscar si este médico tiene un honorario específico para este servicio
                    var customHonorarium = await _context.HonorariosMedicosServicios
                        .FirstOrDefaultAsync(h => h.ServicioId == serviceId.Value && h.MedicoId == finalMedicoId.Value, cancellationToken);

                    decimal honorarioAsignado = customHonorarium?.MontoHonorario ?? detalle.Honorario;

                    detalle.AsignarMedicoResponsable(finalMedicoId.Value, categoriaMapeada ?? HonorarioConstants.CategoriaOtros, honorarioAsignado);
                    
                    // También agregarlo a la lista de múltiples médicos con rol default para consistencia
                    detalle.AgregarMedicoResponsable(finalMedicoId.Value, "Médico Responsable", honorarioAsignado);

                    var medicoNombre = (await _context.Medicos.FindAsync(new object[] { finalMedicoId.Value }, cancellationToken))?.Nombre;
                    _context.LogsAsignacionHonorario.Add(new LogAsignacionHonorario(
                        detalle.Id, request.Descripcion, sourceAccion,
                        null, null, finalMedicoId.Value, medicoNombre,
                        request.UsuarioCarga, sourceAccion == HonorarioConstants.AccionAsignacionDefault ? "Auto-asignado por configuración" : "Asignado durante carga directa"));
                    
                    _logger.LogInformation("Asignado médico responsable {MedicoId} ({Accion}) para detalle {DetalleId}. Honorario: {Honorario}",
                        finalMedicoId.Value, sourceAccion, detalle.Id, honorarioAsignado);
                }
            }
        }

        private async Task<CuentaServicios> GetOrCreateCuentaAsync(Guid pacienteId, CargarServicioACuentaCommand request, CancellationToken ct)
        {
            var cuenta = await _repository.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, ct);
            if (cuenta == null)
            {
                cuenta = new CuentaServicios(pacienteId, request.UsuarioCarga, request.TipoIngreso, request.ConvenioId);
                await _repository.AgregarCuentaAsync(cuenta, ct);
            }
            return cuenta;
        }

        private async Task ProcesarCitaMedicaAsync(CargarServicioACuentaCommand request, Guid pacienteId, Guid cuentaId, CancellationToken ct)
        {
            if (!request.MedicoId.HasValue || !request.HoraCita.HasValue)
                throw new InvalidOperationException("Los servicios de consulta requieren Médico y Hora de Cita.");

            var horaNormalizada = new DateTime(
                request.HoraCita.Value.Year, request.HoraCita.Value.Month, request.HoraCita.Value.Day,
                request.HoraCita.Value.Hour, request.HoraCita.Value.Minute, 0, 
                DateTimeKind.Unspecified);

            if (await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, horaNormalizada, ct))
                throw new InvalidOperationException($"El médico ya tiene una cita pautada para las {horaNormalizada:HH:mm}.");

            var cita = new CitaMedica(request.MedicoId.Value, pacienteId, cuentaId, horaNormalizada);
            await _repository.AgregarCitaMedicaAsync(cita, ct);
        }

        private async Task NotificarSistemasExternosAsync(CargarServicioACuentaCommand request, CancellationToken ct)
        {
            // Senior Logic (V16.2): Las órdenes de imágenes ahora se disparan al CERRAR la cuenta
            // para evitar órdenes huérfanas de servicios no pagados.
            // Se mantiene la notificación de legado para trazabilidad financiera.
            await _externaService.EnviarOrdenLegacyAsync(request.Precio * request.Cantidad, 0, ct);
        }
    }
}
