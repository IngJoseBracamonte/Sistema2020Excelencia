using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.System;
using SistemaSatHospitalario.Core.Application.Queries.System;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Controllers.System
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator,Administrador")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] bool? resueltos)
        {
            var query = new GetTicketsQuery { Resueltos = resueltos };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(Guid id, [FromBody] ResolveTicketRequest request)
        {
            var command = new ResolveTicketCommand
            {
                TicketId = id,
                ComentariosResolucion = request.ComentariosResolucion,
                ResueltoPorUsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            var success = await _mediator.Send(command);

            if (!success)
                return NotFound(new { message = "Ticket no encontrado." });

            return Ok(new { message = "Ticket marcado como resuelto exitosamente." });
        }

        [HttpPost("report")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalReport([FromBody] CreateErrorTicketCommand command, [FromHeader(Name = "X-Testing-Token")] string token)
        {
            // Simple validation to prevent spam (could be improved with a real config value)
            if (token != "S4T_Hosp_Testing_2026") 
                return Unauthorized();

            var ticketId = await _mediator.Send(command);
            return Ok(new { ticketId, message = "Reporte crítico recibido y alertado." });
        }
    }

    public class ResolveTicketRequest
    {
        public string? ComentariosResolucion { get; set; }
    }
}
