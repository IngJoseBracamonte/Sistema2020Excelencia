using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Commands;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = "Admin,Administrador,Asistente Particular,Asistente Seguro,Asistente de Seguros,Asistente RX,Médico,Cajero")]
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BillingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CargarServicio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarServicio([FromBody] CargarServicioACuentaCommand command)
        {
            try
            {
                var idCuenta = await _mediator.Send(command);
                return Ok(new { Message = "Servicio cargado exitosamente a la cuenta.", CuentaId = idCuenta });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
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
        public async Task<IActionResult> RemoveServicio([FromQuery] Guid cuentaId, [FromQuery] Guid servicioId)
        {
            try
            {
                var command = new RemoveServicioDeCuentaCommand { CuentaId = cuentaId, ServicioId = servicioId };
                await _mediator.Send(command);
                return Ok(new { Message = "Servicio removido exitosamente de la cuenta." });
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
                command.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonimo";
                var exito = await _mediator.Send(command);
                if (exito) return Ok(new { Message = "Turno reservado temporalmente." });
                return BadRequest(new { Error = "El turno ya no está disponible o ha sido reservado por otro usuario." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("BloquearHorario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrador")]
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
