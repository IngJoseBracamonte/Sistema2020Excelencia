using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
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
        public string? OrigenCarga { get; set; } // "Enfermeria", "Hospitalizacion", "UCI", "Emergencia", etc.

        // Datos para Cita Médica (solo si TipoServicio == "Medico")
        public Guid? MedicoId { get; set; }
        public DateTime? HoraCita { get; set; }
        public Guid? AreaClinicaId { get; set; }

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
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly ILogger<CargarServicioACuentaCommandHandler> _logger;

        public CargarServicioACuentaCommandHandler(
            IBillingRepository repository, 
            IOrdenExternaService externaService, 
            IApplicationDbContext context, 
            IHonorariumMapperService mapperService, 
            IInventoryService inventoryService,
            ILegacyLabRepository legacyRepository,
            ILogger<CargarServicioACuentaCommandHandler> logger)
        {
            _repository = repository;
            _externaService = externaService;
            _context = context;
            _mapperService = mapperService;
            _inventoryService = inventoryService;
            _legacyRepository = legacyRepository;
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
                bool requiresAppointmentTime = request.TipoIngreso == EstadoConstants.Particular || request.TipoIngreso == EstadoConstants.Seguro;
                if (requiresAppointmentTime || (request.MedicoId.HasValue && request.HoraCita.HasValue))
                {
                    Guid? citaAreaClinicaId = request.AreaClinicaId;
                    if (!string.IsNullOrEmpty(request.OrigenCarga) && request.OrigenCarga.StartsWith("ENFERMERIA", StringComparison.OrdinalIgnoreCase))
                    {
                        var areaEnf = await _context.AreasClinicas.FirstOrDefaultAsync(a => a.Codigo == "ENFERMERIA", cancellationToken);
                        if (areaEnf == null)
                        {
                            areaEnf = new AreaClinica(SeedConstants.SedeId_Principal, "ENFERMERIA", "Enfermería");
                            _context.AreasClinicas.Add(areaEnf);
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                        citaAreaClinicaId = areaEnf.Id;
                    }

                    if (!request.MedicoId.HasValue || !request.HoraCita.HasValue)
                        throw new InvalidOperationException("Los servicios de consulta requieren Médico y Hora de Cita.");

                    var horaNormalizada = new DateTime(
                        request.HoraCita.Value.Year, request.HoraCita.Value.Month, request.HoraCita.Value.Day,
                        request.HoraCita.Value.Hour, request.HoraCita.Value.Minute, 0, 
                        DateTimeKind.Unspecified);

                    while (await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, horaNormalizada, cancellationToken))
                    {
                        horaNormalizada = horaNormalizada.AddMinutes(1);
                    }

                    var cita = new CitaMedica(request.MedicoId.Value, paciente.Id, cuenta.Id, horaNormalizada, null, citaAreaClinicaId);
                    await _repository.AgregarCitaMedicaAsync(cita, cancellationToken);
                }
            }

            // Senior Enrichment: Capturar LegacyMappingId del catálogo (V12.2)
            string? legacyId = null;
            bool esLab = EstadoConstants.EsLaboratorio(request.TipoServicio) || (baseService != null && baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Laboratory);
            bool esRx = baseService != null && (baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Radiology || baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Tomography) || request.TipoServicio == EstadoConstants.RX || request.TipoServicio == EstadoConstants.TOMO;

            // Regla Polimórfica de Cantidad: Consulta, Laboratorio y RX son unidades unitarias estrictas (1)
            decimal finalCantidad = (esConsulta || esLab || esRx) ? 1m : Math.Max(1m, request.Cantidad);

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
                decimal basePrice = baseService?.PrecioBase ?? 0;
                if (request.ConvenioId.HasValue)
                {
                    if (esLab)
                    {
                        if (int.TryParse(request.ServicioId, out int perfilId))
                        {
                            var exc = await _context.ConvenioPerfilPrecios
                                .FirstOrDefaultAsync(x => x.SeguroConvenioId == request.ConvenioId.Value && x.PerfilId == perfilId, cancellationToken);
                            if (exc != null && exc.PrecioUSD > 0)
                            {
                                basePrice = exc.PrecioUSD;
                            }
                        }
                    }
                    else if (baseService != null)
                    {
                        var priceConv = await _context.PreciosServicioConvenio
                             .FirstOrDefaultAsync(p => p.SeguroConvenioId == request.ConvenioId.Value && p.ServicioClinicoId == baseService.Id, cancellationToken);
                        if (priceConv != null)
                        {
                            basePrice = priceConv.PrecioDiferencial;
                        }
                    }
                }

                decimal doctorHonorary = 0;
                if (baseService != null)
                {
                    if (esConsulta)
                    {
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
                    }
                    else
                    {
                        if (request.MedicoId.HasValue)
                        {
                            var serviceGuid = Guid.TryParse(request.ServicioId, out var parsedGuid) ? parsedGuid : Guid.Empty;
                            var customHonorarium = await _context.HonorariosMedicosServicios
                                 .FirstOrDefaultAsync(h => h.ServicioId == serviceGuid && h.MedicoId == request.MedicoId.Value, cancellationToken);
                            doctorHonorary = customHonorarium?.MontoHonorario ?? baseService.HonorarioBase;
                        }
                        else
                        {
                            doctorHonorary = baseService.HonorarioBase;
                        }
                    }
                }

                if (baseService != null || esLab)
                {
                    if (request.Precio == basePrice && (request.Honorario == 0 || (baseService != null && request.Honorario == baseService.HonorarioBase)))
                    {
                        finalPrecio = basePrice + doctorHonorary;
                    }

                    if (request.Honorario == 0)
                    {
                        finalHonorario = doctorHonorary;
                    }
                    else if (request.Honorario == doctorHonorary + basePrice)
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
                finalCantidad, 
                request.TipoServicio, 
                request.UsuarioCarga,
                legacyId,
                request.AreaClinicaId);

            if (_context.DetallesServicioCuenta != null)
            {
                _context.DetallesServicioCuenta.Add(detalle);
            }

            // Auto-asignación de Médico Responsable y honorarios
            await AsignarMedicosYHonorariosAsync(request, detalle, esConsulta, cancellationToken);

            // 5. Notificaciones e Integraciones Externas
            await NotificarSistemasExternosAsync(request, cancellationToken);

            // Generar Orden Inmediata de RX/TOMO si viene de un módulo de carga clínica (como enfermería, hospitalización, uci)
            if (esRx && !string.IsNullOrEmpty(request.OrigenCarga))
            {
                string nombrePaciente = paciente.NombreCompleto ?? paciente.NombreCorto ?? "Paciente Desconocido";
                if (request.TipoServicio == EstadoConstants.RX || (baseService != null && baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Radiology))
                {
                    await _externaService.EnviarOrdenRXAsync(cuenta.Id, paciente.Id, request.Descripcion, nombrePaciente, cancellationToken);
                }
                else
                {
                    await _externaService.EnviarOrdenTomoAsync(cuenta.Id, paciente.Id, request.Descripcion, nombrePaciente, cancellationToken);
                }
            }

            // Generar Orden Inmediata de Laboratorio Legacy si viene de un módulo de carga clínica (como enfermería, hospitalización, uci)
            if (esLab && !string.IsNullOrEmpty(request.OrigenCarga) && int.TryParse(legacyId, out int idPerfil))
            {
                _logger.LogInformation("[LEGACY-SYNC-IMMEDIATE] Generando orden de laboratorio legacy inmediata para perfil {PerfilId}...", idPerfil);
                
                if (!paciente.IdPacienteLegacy.HasValue || paciente.IdPacienteLegacy.Value == 0)
                {
                    var existingLegacy = await _legacyRepository.GetPatientByCedulaAsync(paciente.CedulaPasaporte, cancellationToken);
                    if (existingLegacy != null)
                    {
                        paciente.VincularLegacy(existingLegacy.IdPersona);
                    }
                    else
                    {
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
                        int newId = await _legacyRepository.CreatePatientLegacyAsync(legacyPatient, cancellationToken);
                        if (newId > 0)
                        {
                            paciente.VincularLegacy(newId);
                        }
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (paciente.IdPacienteLegacy.HasValue && paciente.IdPacienteLegacy.Value > 0)
                {
                    var perfilesFacturados = new List<PerfilesFacturadosLegacy>
                    {
                        new PerfilesFacturadosLegacy
                        {
                            IdOrden = 0,
                            IdPersona = paciente.IdPacienteLegacy.Value,
                            IdPerfil = idPerfil,
                            PrecioPerfil = finalPrecio * finalCantidad
                        }
                    };

                    var ordenLegacy = new OrdenLegacy
                    {
                        IdPersona = paciente.IdPacienteLegacy.Value,
                        IDConvenio = request.ConvenioId ?? cuenta.ConvenioId ?? 1,
                        Fecha = DateTime.Now,
                        HoraIngreso = DateTime.Now.ToString("HH:mm:ss"),
                        PrecioF = perfilesFacturados.Sum(p => p.PrecioPerfil)
                    };

                    var idOrdenLegacy = await _legacyRepository.GenerarOrdenLaboratorioAsync(ordenLegacy, perfilesFacturados, new List<ResultadosPacienteLegacy>(), cancellationToken);
                    _logger.LogInformation("[LEGACY-SYNC-IMMEDIATE] Orden de laboratorio legacy generada exitosamente con ID: {IdOrden}", idOrdenLegacy);
                }
            }

            // Deducir stock del almacén de la zona de origen si viene especificado
            Guid? targetSedeId = null;
            if (!string.IsNullOrEmpty(request.OrigenCarga))
            {
                string zonaLimpia = request.OrigenCarga;
                if (zonaLimpia.StartsWith("Enfermeria_", StringComparison.OrdinalIgnoreCase))
                {
                    zonaLimpia = zonaLimpia.Substring("Enfermeria_".Length);
                }

                targetSedeId = zonaLimpia.ToUpperInvariant() switch
                {
                    "EMERGENCIA" => SeedConstants.SedeId_Emergencia,
                    "HOSPITALIZACION" or "HOSPITALIZACIÓN" => SeedConstants.SedeId_Hospitalizacion,
                    "UCI" => SeedConstants.SedeId_UCI,
                    _ => null
                };

                if (targetSedeId == null && (zonaLimpia.Equals("ENFERMERIA", StringComparison.OrdinalIgnoreCase) || zonaLimpia.Equals("ENFERMERÍA", StringComparison.OrdinalIgnoreCase)))
                {
                    var sedeEnf = await _context.Sedes.FirstOrDefaultAsync(s => s.Codigo == "ENFERMERIA" || s.Nombre.Contains("Enfermer"), cancellationToken);
                    targetSedeId = sedeEnf?.Id ?? SeedConstants.SedeId_Hospitalizacion;
                }
            }

            if (targetSedeId == null)
            {
                targetSedeId = cuenta.AreaClinicaId.HasValue
                    ? (await _context.AreasClinicas.FirstOrDefaultAsync(a => a.Id == cuenta.AreaClinicaId.Value, cancellationToken))?.SedeId
                    : SeedConstants.ResolveSedeInventario(cuenta.TipoIngreso, cuenta.SubAreaClinica);
            }

            // Deduct stock for inventory items associated with this service
            await _inventoryService.DeductInventoryForServiceDetailAsync(
                detalle.Id,
                detalle.ServicioId,
                baseService?.Codigo ?? string.Empty,
                detalle.Descripcion,
                detalle.Cantidad,
                request.UsuarioCarga,
                cuenta.Id,
                targetSedeId,
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

            bool esLab = EstadoConstants.EsLaboratorio(request.TipoServicio) || (baseService != null && baseService.Category == SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Laboratory);

            decimal basePrice = baseService.PrecioBase;
            if (request.ConvenioId.HasValue)
            {
                if (esLab)
                {
                    if (int.TryParse(request.ServicioId, out int perfilId))
                    {
                        var exc = await _context.ConvenioPerfilPrecios
                            .FirstOrDefaultAsync(x => x.SeguroConvenioId == request.ConvenioId.Value && x.PerfilId == perfilId, cancellationToken);
                        if (exc != null && exc.PrecioUSD > 0)
                        {
                            basePrice = exc.PrecioUSD;
                        }
                    }
                }
                else
                {
                    var priceConv = await _context.PreciosServicioConvenio
                        .FirstOrDefaultAsync(p => p.SeguroConvenioId == request.ConvenioId.Value && p.ServicioClinicoId == baseService.Id, cancellationToken);
                    if (priceConv != null)
                    {
                        basePrice = priceConv.PrecioDiferencial;
                    }
                }
            }

            decimal expectedPrecio = basePrice;
            decimal doctorHonorary = 0;
            if (esConsulta)
            {
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
                expectedPrecio = basePrice + doctorHonorary;
            }
            else
            {
                if (request.MedicoId.HasValue)
                {
                    var serviceGuid = Guid.TryParse(request.ServicioId, out var parsedGuid) ? parsedGuid : Guid.Empty;
                    var customHonorarium = await _context.HonorariosMedicosServicios
                        .FirstOrDefaultAsync(h => h.ServicioId == serviceGuid && h.MedicoId == request.MedicoId.Value, cancellationToken);
                    doctorHonorary = customHonorarium?.MontoHonorario ?? baseService.HonorarioBase;
                }
                else
                {
                    doctorHonorary = baseService.HonorarioBase;
                }
                expectedPrecio = basePrice + doctorHonorary;
            }

            decimal checkPrecio = request.PrecioModificado ?? request.Precio;
            if (checkPrecio != expectedPrecio && checkPrecio != basePrice)
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
                    detalle.AgregarMedicoResponsable(finalMedicoId.Value, HonorarioConstants.RolMedicoResponsable, honorarioAsignado);

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

            while (await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, horaNormalizada, ct))
            {
                horaNormalizada = horaNormalizada.AddMinutes(1);
            }

            var cita = new CitaMedica(request.MedicoId.Value, pacienteId, cuentaId, horaNormalizada, null, request.AreaClinicaId);
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
