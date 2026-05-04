using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admin;
using SistemaSatHospitalario.Core.Application.Queries.Admin;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles)]
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionHonorariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AsignacionHonorariosController(IMediator mediator) { _mediator = mediator; }

        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? estado)
        {
            var result = await _mediator.Send(new GetServiciosSinAsignarQuery
            {
                FechaDesde = desde, FechaHasta = hasta, EstadoFiltro = estado ?? "TODOS"
            });
            return Ok(result);
        }

        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar([FromBody] AsignarMedicoAServicioCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Médico asignado correctamente" });
        }
    }
}
