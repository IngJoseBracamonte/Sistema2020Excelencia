using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = "Administrador,Asistente Particular,Asistente de Seguros,Cajero")]
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
    }
}
