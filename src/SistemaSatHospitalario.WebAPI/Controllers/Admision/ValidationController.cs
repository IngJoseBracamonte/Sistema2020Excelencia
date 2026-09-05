using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ValidationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene la lista de validaciones pendientes filtradas por el rol del usuario actual.
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<ValidationDashboardDto>> GetDashboard([FromQuery] DateTime? fecha)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
            
            var query = new GetTechnicalValidationListQuery 
            { 
                Role = userRole,
                Fecha = fecha 
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Realiza la validación técnica de un servicio o cita.
        /// </summary>
        [HttpPost("execute")]
        public async Task<ActionResult<bool>> Validate([FromBody] ValidateTechnicalServiceCommand command)
        {
            // [SEC] Enforce the current user as the operator
            command.UsuarioOperador = User.Identity?.Name ?? "Operador_Desconocido";
            
            var result = await _mediator.Send(command);
            
            if (!result) return BadRequest(new { message = "No se pudo realizar la validación. Verifique el ID." });
            
            return Ok(result);
        }
    }
}
