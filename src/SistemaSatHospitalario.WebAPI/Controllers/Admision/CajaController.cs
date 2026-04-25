using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CajaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Abre la Caja Diaria para el usuario actual. (Uso manual restringido a Administradores).
        /// </summary>
        [HttpPost("Abrir")]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaCommand command)
        {
            try
            {
                command.UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                command.NombreUsuario = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "Usuario";

                var idCaja = await _mediator.Send(command);
                return Ok(new { Message = "Caja abierta con éxito.", CajaId = idCaja });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Cierra la caja activa del usuario actual.
        /// </summary>
        [HttpPost("Cerrar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CerrarCaja()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var result = await _mediator.Send(new CerrarCajaCommand { UsuarioId = usuarioId });
                return Ok(new { Message = "Caja cerrada con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial de cierres para administración (Totales y por Usuario).
        /// </summary>
        [HttpGet("Historial")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerHistorial([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? usuarioId)
        {
            var query = new GetCajaSummariesQuery 
            { 
                Desde = desde ?? DateTime.UtcNow.AddDays(-7),
                Hasta = hasta ?? DateTime.UtcNow,
                UsuarioId = usuarioId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Resumen")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerResumen()
        {
            var resumen = await _mediator.Send(new ObtenerResumenCajasQuery());
            return Ok(resumen);
        }

        [HttpGet("PersonalReport")]
        public async Task<ActionResult<DailyClosingDto>> GetPersonalReport([FromQuery] string? userId)
        {
            var id = userId ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var query = new GetDailyClosingQuery { UserId = id, Fecha = DateTime.Today };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
