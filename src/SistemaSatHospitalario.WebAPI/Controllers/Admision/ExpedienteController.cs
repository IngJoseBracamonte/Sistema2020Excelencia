using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpedienteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpedienteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("billing")]
        public async Task<IActionResult> GetBillingExpediente([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string searchTerm)
        {
            var result = await _mediator.Send(new GetExpedienteFacturacionQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate, 
                SearchTerm = searchTerm 
            });
            return Ok(result);
        }

        [HttpGet("citas")]
        public async Task<IActionResult> GetControlCitas([FromQuery] DateTime? date)
        {
            var result = await _mediator.Send(new GetControlCitasQuery 
            { 
                Date = date ?? DateTime.Today 
            });
            return Ok(result);
        }

        [HttpPost("citas/{id}/atender")]
        public async Task<IActionResult> MarkAtendida(Guid id)
        {
            var result = await _mediator.Send(new MarkAppointmentAtendidaCommand { AppointmentId = id });
            return result ? Ok() : NotFound();
        }

        [HttpPost("citas/{id}/cancelar")]
        public async Task<IActionResult> CancelCita(Guid id)
        {
            var result = await _mediator.Send(new AdminCancelAppointmentCommand { AppointmentId = id });
            return result ? Ok() : NotFound();
        }
    }
}
