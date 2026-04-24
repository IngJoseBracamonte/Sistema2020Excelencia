using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReceivablesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReceivablesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Pending")]
        public async Task<ActionResult<List<PendingARDto>>> GetPendingAR(
            [FromQuery] string? searchTerm, 
            [FromQuery] string? estado,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            // Senior Fix: Si el parámetro es nulo, blanco o "Todas", no filtramos por estado en el handler.
            string? filterEstado = string.IsNullOrWhiteSpace(estado) || estado.Equals("Todas", StringComparison.OrdinalIgnoreCase)
                                   ? null 
                                   : estado;
            
            var query = new GetPendingARQuery 
            { 
                SearchTerm = searchTerm, 
                Estado = filterEstado,
                StartDate = startDate,
                EndDate = endDate
            };
            var results = await _mediator.Send(query);
            return Ok(results);
        }

        [HttpPost("Settle")]
        public async Task<IActionResult> SettleAR([FromBody] SettleARCommand command)
        {
            try
            {
                command.UsuarioCarga = User.Identity?.Name ?? "Sistama";
                var success = await _mediator.Send(command);
                return Ok(new { Message = "Cobro procesado exitosamente.", Success = success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("Audit")]
        public async Task<IActionResult> AuditAR([FromBody] AuditARCommand command)
        {
            try
            {
                command.UsuarioAuditor = User.Identity?.Name ?? "Sistama";
                var success = await _mediator.Send(command);
                return Ok(new { Message = "Auditoría procesada exitosamente.", Success = success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
