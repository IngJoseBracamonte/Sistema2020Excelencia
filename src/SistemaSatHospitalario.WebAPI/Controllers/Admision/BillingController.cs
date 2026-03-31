using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Commands;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.WebAPI.Infrastructure.Security;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = "Admin,Administrador,Asistente Particular,Asistente Seguro,Asistente de Seguros,Asistente RX,Médico,Cajero")]
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BillingController> _logger;
 
        public BillingController(IMediator mediator, ILogger<BillingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("CargarServicio")]
        [Idempotent]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarServicio([FromBody] CargarServicioACuentaCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Servicio cargado exitosamente.", 
                    CuentaId = result.CuentaId,
                    DetalleId = result.DetalleId
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validación de negocio fallida en CargarServicio");
                return BadRequest(new { Error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al cargar servicio");
                return BadRequest(new { Error = "Error de integridad de datos. Verifique que los IDs de paciente y servicio sean correctos.", Details = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en CargarServicio");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Ha ocurrido un error inesperado al procesar el servicio." });
            }
        }
        
        [HttpPost("SincronizarCarrito")]
        [Idempotent]
        [ActionName("SincronizarCarrito")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SincronizarCarrito([FromBody] SyncCarritoCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Carrito sincronizado exitosamente.", 
                    CuentaId = result.CuentaId,
                    Detalles = result.Detalles
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validación de negocio fallida en SincronizarCarrito");
                return BadRequest(new { Error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de persistencia masiva en SincronizarCarrito");
                return BadRequest(new { Error = "Error de persistencia masiva/integridad. Detalle: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo crítico en SincronizarCarrito");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Error crítico durante la sincronización masiva." });
            }
        }

        [HttpPost("CloseAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CloseAccount([FromBody] CloseAccountCommand command)
        {
            try
            {
                // Auto-poblar identidad del cajero desde el token JWT (Micro-Ciclo 28)
                command.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                command.UsuarioCajero = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "Cajero Desconocido";

                var reciboId = await _mediator.Send(command);
                return Ok(new { Message = "Cuenta cerrada y facturada exitosamente.", ReciboId = reciboId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("RemoveServicio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveServicio([FromQuery] Guid cuentaId, [FromQuery] Guid detalleId, [FromQuery] Guid? medicoId = null, [FromQuery] DateTime? horaCita = null)
        {
            try
            {
                var command = new RemoveServicioDeCuentaCommand 
                { 
                    CuentaId = cuentaId, 
                    DetalleId = detalleId,
                    MedicoId = medicoId,
                    HoraCita = horaCita
                };
                await _mediator.Send(command);
                return Ok(new { Message = "Servicio removido exitosamente de la cuenta." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("CancelAppointment/{appointmentId}")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            try
            {
                var command = new AdminCancelAppointmentCommand { AppointmentId = appointmentId };
                await _mediator.Send(command);
                return Ok(new { Message = "Cita anulada administrativamente exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("Appointments")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<IActionResult> GetAppointments([FromQuery] DateTime? fecha, [FromQuery] Guid? medicoId)
        {
            try
            {
                var query = new GetActiveAppointmentsQuery { Fecha = fecha, MedicoId = medicoId };
                var results = await _mediator.Send(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("ReservarTurno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReservarTurno([FromBody] ReservarTurnoTemporalCommand command)
        {
            try
            {
                command.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "Anonimo";
                await _mediator.Send(command);
                return Ok(new { Message = "Turno reservado temporalmente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost("BloquearHorario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize] // Permite a cualquier usuario autenticado (Cajero, Asistente, etc.)
        public async Task<IActionResult> BloquearHorario([FromBody] BloquearHorarioCommand command)
        {
            try
            {
                var exito = await _mediator.Send(command);
                if (exito) return Ok(new { Message = "Horario bloqueado administrativamente." });
                return BadRequest(new { Error = "No se pudo bloquear el horario (podría tener una cita activa)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
