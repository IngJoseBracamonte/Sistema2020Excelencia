using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Hubs;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImagingController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IHonorariumMapperService _mapperService;
        private readonly INotificationService _notificationService;

        public ImagingController(
            IApplicationDbContext context, 
            IHubContext<DashboardHub> hubContext, 
            IHonorariumMapperService mapperService,
            INotificationService notificationService)
        {
            _context = context;
            _hubContext = hubContext;
            _mapperService = mapperService;
            _notificationService = notificationService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders([FromQuery] string type)
        {
            // type debe ser RX o TOMO
            var orders = await _context.OrdenesImagenes
                .Where(o => o.TipoServicio == type && o.Estado == "Pendiente")
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] string? type,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? search)
        {
            var query = _context.OrdenesImagenes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(type) && !type.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => o.TipoServicio == type);
            }

            if (!string.IsNullOrEmpty(status) && !status.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => o.Estado == status);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                var start = startDate?.Date ?? DateTime.MinValue;
                var end = endDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;
                query = query.Where(o => o.FechaCreacion >= start && o.FechaCreacion <= end);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                var pacIds = await _context.PacientesAdmision
                    .Where(p => p.CedulaPasaporte.Contains(searchLower))
                    .Select(p => p.Id)
                    .ToListAsync();

                query = query.Where(o => o.PacienteNombre.ToLower().Contains(searchLower) || 
                                         o.Estudio.ToLower().Contains(searchLower) || 
                                         pacIds.Contains(o.PacienteId));
            }

            var orders = await query
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.OrdenesImagenes.FindAsync(id);
            if (order == null) return NotFound(new { Message = "Orden no encontrada." });

            var usuario = User.Identity?.Name ?? "Sistema";
            order.MarcarComoProcesado(usuario);

            // ═══ Cerrar ciclo de Honorarios: Marcar DetalleServicioCuenta como Realizado ═══
            // Buscar el detalle de facturación vinculado por CuentaId + Estudio (descripción).
            if (order.CuentaId != Guid.Empty)
            {
                var detalle = await _context.DetallesServicioCuenta
                    .FirstOrDefaultAsync(d => d.CuentaServicioId == order.CuentaId
                                           && d.Descripcion == order.Estudio
                                           && !d.Realizado);

                if (detalle != null)
                {
                    // 1. Marcar como realizado (el asistente aceptó el estudio)
                    detalle.MarcarRealizado(usuario);

                    // 2. Si no tiene médico asignado, auto-asignar desde HonorarioConfig
                    if (detalle.MedicoResponsableId == null && detalle.Honorario > 0)
                    {
                        var categoria = await _mapperService.MapToCategoryAsync(order.TipoServicio);
                        if (categoria != HonorarioConstants.CategoriaOtros)
                        {
                            var config = await _context.HonorariosConfig
                                .FirstOrDefaultAsync(h => h.CategoriaServicio == categoria);
                            if (config?.MedicoDefaultId != null)
                            {
                                detalle.AsignarMedicoResponsable(config.MedicoDefaultId.Value, categoria);

                                var medicoNombre = (await _context.Medicos.FindAsync(config.MedicoDefaultId.Value))?.Nombre;
                                _context.LogsAsignacionHonorario.Add(new LogAsignacionHonorario(
                                    detalle.Id, detalle.Descripcion, HonorarioConstants.AccionAsignacionDefault,
                                    null, null, config.MedicoDefaultId.Value, medicoNombre,
                                    usuario, "Auto-asignado al procesar orden de imagen"));
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(default);

            // Broadcast real vía SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = order.Id,
                status = order.Estado,
                patientName = order.PacienteNombre,
                servicioNombre = order.Estudio,
                tipoServicio = order.TipoServicio
            });

            return Ok(new { Message = "Orden marcada como procesada." });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.OrdenesImagenes.FindAsync(id);
            if (order == null) return NotFound(new { Message = "Orden no encontrada." });

            var usuario = User.Identity?.Name ?? "Sistema";
            order.Estado = "Anulado";
            order.ProcesadoPor = usuario;
            order.FechaProcesado = DateTime.UtcNow;

            await _context.SaveChangesAsync(default);

            // Broadcast real vía SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = order.Id,
                status = order.Estado,
                patientName = order.PacienteNombre,
                servicioNombre = order.Estudio,
                tipoServicio = order.TipoServicio
            });

            return Ok(new { Message = "Orden marcada como anulada." });
        }

        [HttpPost("direct")]
        public async Task<IActionResult> CreateDirectOrder([FromBody] CreateDirectOrderRequest request)
        {
            if (request == null) return BadRequest(new { Message = "Datos inválidos." });
            if (string.IsNullOrEmpty(request.PacienteNombre) || request.Estudios == null || request.Estudios.Count == 0 || string.IsNullOrEmpty(request.TipoServicio))
            {
                return BadRequest(new { Message = "El nombre del paciente, al menos un estudio y el tipo de servicio son obligatorios." });
            }

            var usuario = User.Identity?.Name ?? "Sistema";
            var category = await _mapperService.MapToCategoryAsync(request.TipoServicio);
            Guid? defaultMedicoId = null;
            string? defaultMedicoNombre = null;

            if (category != HonorarioConstants.CategoriaOtros)
            {
                var config = await _context.HonorariosConfig
                    .FirstOrDefaultAsync(h => h.CategoriaServicio == category);
                if (config?.MedicoDefaultId != null)
                {
                    defaultMedicoId = config.MedicoDefaultId;
                    var defaultMedico = await _context.Medicos.FindAsync(defaultMedicoId.Value);
                    defaultMedicoNombre = defaultMedico?.Nombre;
                }
            }

            var createdOrders = new List<OrdenImagen>();

            foreach (var estudio in request.Estudios)
            {
                var order = new OrdenImagen
                {
                    CuentaId = Guid.Empty, // Se asociará al validar
                    PacienteId = request.PacienteId,
                    PacienteNombre = request.PacienteNombre,
                    Estudio = estudio.Trim(),
                    TipoServicio = request.TipoServicio.ToUpper(),
                    Estado = "Pendiente", // Comienza como Pendiente para ser procesado tras la aprobación
                    FechaCreacion = DateTime.UtcNow,
                    EsDirecta = true,
                    RequiereValidacion = true,
                    Validada = false,
                    MedicoSolicitanteId = defaultMedicoId,
                    MedicoSolicitanteNombre = defaultMedicoNombre
                };

                _context.OrdenesImagenes.Add(order);
                createdOrders.Add(order);
            }

            await _context.SaveChangesAsync(default);

            foreach (var order in createdOrders)
            {
                // 1. Crear Notificación Persistente para Administradores, Supervisores y Asistente de Seguros
                await _notificationService.CreatePersistentNotificationAsync(
                    $"Nueva Orden Directa de RX: {order.PacienteNombre}",
                    $"Se requiere validación del estudio de RX '{order.Estudio}' registrado por la estación.",
                    "Warning",
                    targetUserId: null,
                    targetRole: null,
                    actionUrl: "/admin/audit/cuentas?tab=ordenes-directas",
                    ct: default
                );

                // 2. Alertar al grupo en tiempo real
                await _notificationService.SendValidationAlertAsync(
                    $"Nueva Orden Directa de RX ({order.PacienteNombre})",
                    $"Se requiere validación administrativa para el estudio de RX '{order.Estudio}' del paciente {order.PacienteNombre}.",
                    "RX",
                    new { orderId = order.Id },
                    default
                );

                // Broadcast SignalR update para actualizar la Estación de RX
                await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                    orderId = order.Id,
                    status = order.Estado,
                    patientName = order.PacienteNombre,
                    servicioNombre = order.Estudio,
                    tipoServicio = order.TipoServicio,
                    esDirecta = true,
                    requiereValidacion = true
                });
            }

            return Ok(new { Message = "Orden directa registrada con éxito, pendiente de validación administrativa." });
        }

        [HttpGet("pending-validation")]
        [Authorize(Roles = "Admin,Administrador,Supervisor,Asistente Seguro,Asistente de Seguros")]
        public async Task<IActionResult> GetPendingValidation([FromQuery] string? type)
        {
            var query = _context.OrdenesImagenes.AsNoTracking()
                .Where(o => o.EsDirecta && o.RequiereValidacion && !o.Validada);

            if (!string.IsNullOrEmpty(type) && !type.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => o.TipoServicio == type);
            }

            var orders = await query
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetImagingServices([FromQuery] string type)
        {
            if (string.IsNullOrEmpty(type)) return BadRequest(new { Message = "El tipo de servicio es obligatorio." });
            var normalizedType = type.ToUpper();
            
            var services = await _context.ServiciosClinicos.AsNoTracking()
                .Where(s => s.Activo && s.TipoServicio == normalizedType)
                .OrderBy(s => s.Descripcion)
                .Select(s => new {
                    s.Id,
                    s.Codigo,
                    s.Descripcion,
                    s.PrecioBase,
                    s.HonorarioBase,
                    s.TipoServicio
                })
                .ToListAsync();

            return Ok(services);
        }

        [HttpPost("{id}/validate-direct")]
        [Authorize(Roles = "Admin,Administrador,Supervisor,Asistente Seguro,Asistente de Seguros")]
        public async Task<IActionResult> ValidateDirectOrder(int id, [FromBody] ValidateDirectOrderRequest request)
        {
            if (request == null) return BadRequest(new { Message = "Datos de validación inválidos." });

            var order = await _context.OrdenesImagenes.FindAsync(id);
            if (order == null) return NotFound(new { Message = "Orden no encontrada." });
            if (!order.EsDirecta || !order.RequiereValidacion) return BadRequest(new { Message = "La orden especificada no es una orden directa que requiera validación." });
            if (order.Validada) return BadRequest(new { Message = "Esta orden directa ya fue validada anteriormente." });

            var servicio = await _context.ServiciosClinicos.FindAsync(request.ServicioId);
            if (servicio == null) return BadRequest(new { Message = "El servicio clínico especificado no existe en el catálogo." });

            var usuario = User.Identity?.Name ?? "Sistema";

            CuentaServicios? cuenta = null;
            if (request.CuentaId.HasValue && request.CuentaId.Value != Guid.Empty)
            {
                cuenta = await _context.CuentasServicios.Include(c => c.Detalles)
                    .FirstOrDefaultAsync(c => c.Id == request.CuentaId.Value);
                if (cuenta == null) return NotFound(new { Message = "La cuenta especificada no existe." });
                if (cuenta.Estado != EstadoConstants.Abierta) return BadRequest(new { Message = "La cuenta seleccionada no está abierta." });
            }
            else
            {
                // Buscar cuenta abierta del paciente
                cuenta = await _context.CuentasServicios.Include(c => c.Detalles)
                    .FirstOrDefaultAsync(c => c.PacienteId == order.PacienteId && c.Estado == EstadoConstants.Abierta);

                if (cuenta == null)
                {
                    // Crear nueva cuenta si no hay una abierta
                    cuenta = new CuentaServicios(order.PacienteId, usuario, request.TipoIngreso ?? EstadoConstants.Particular, request.ConvenioId);
                    _context.CuentasServicios.Add(cuenta);
                }
            }

            // Registrar el servicio en la cuenta
            var precio = request.Precio ?? servicio.PrecioBase;
            var honorario = request.Honorario ?? servicio.HonorarioBase;

            var detalle = cuenta.AgregarServicio(
                servicio.Id,
                servicio.Descripcion,
                precio,
                honorario,
                1,
                order.TipoServicio,
                usuario,
                servicio.LegacyMappingId
            );

            // Sincronizar el nombre del estudio de la orden con la descripción del catálogo oficial
            order.Estudio = servicio.Descripcion;

            // Asignar el médico solicitante al detalle para cálculo de honorarios
            var medicoId = request.MedicoSolicitanteId ?? order.MedicoSolicitanteId;
            var medicoNombre = request.MedicoSolicitanteNombre ?? order.MedicoSolicitanteNombre;

            if (medicoId.HasValue)
            {
                var category = await _mapperService.MapToCategoryAsync(order.TipoServicio, servicio.Id);
                detalle.AsignarMedicoResponsable(medicoId.Value, category);

                if (string.IsNullOrEmpty(medicoNombre))
                {
                    var mObj = await _context.Medicos.FindAsync(medicoId.Value);
                    medicoNombre = mObj?.Nombre;
                }

                // Registrar en log de asignación de honorario
                _context.LogsAsignacionHonorario.Add(new LogAsignacionHonorario(
                    detalle.Id,
                    servicio.Descripcion,
                    HonorarioConstants.AccionAsignacionManual,
                    null, null,
                    medicoId.Value,
                    medicoNombre,
                    usuario,
                    "Asignado en validación de orden directa por el administrador"
                ));
            }

            // Actualizar estado de la orden
            order.Validada = true;
            order.ValidadorPor = usuario;
            order.FechaValidacion = DateTime.UtcNow;
            order.CuentaId = cuenta.Id; // Vincular la orden a la cuenta procesada

            await _context.SaveChangesAsync(default);

            return Ok(new { 
                Message = "Orden directa validada y cargada a cuenta con éxito.",
                CuentaId = cuenta.Id,
                DetalleId = detalle.Id
            });
        }
    }

    public class CreateDirectOrderRequest
    {
        public Guid PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public List<string> Estudios { get; set; } = new List<string>();
        public string TipoServicio { get; set; } = string.Empty; // RX o TOMO
    }

    public class ValidateDirectOrderRequest
    {
        public Guid? CuentaId { get; set; }
        public Guid ServicioId { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Honorario { get; set; }
        public string? TipoIngreso { get; set; } // Particular, Seguro, etc.
        public int? ConvenioId { get; set; }
        public Guid? MedicoSolicitanteId { get; set; }
        public string? MedicoSolicitanteNombre { get; set; }
    }
}
