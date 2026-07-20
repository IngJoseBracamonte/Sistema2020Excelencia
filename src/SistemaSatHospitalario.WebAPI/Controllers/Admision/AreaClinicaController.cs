using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Infrastructure.Hubs;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AreaClinicaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<DashboardHub> _hubContext;

        public AreaClinicaController(IMediator mediator, IHubContext<DashboardHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBySede([FromQuery] Guid? sedeId)
        {
            var result = await _mediator.Send(new GetAreasClinicasQuery { SedeId = sedeId });
            return Ok(result);
        }

        [HttpGet("monitoreo")]
        public async Task<IActionResult> GetCamasMonitoreo()
        {
            var result = await _mediator.Send(new GetCamasMonitoreoQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAreaClinicaCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPost("traslado")]
        public async Task<IActionResult> TrasladarPaciente([FromBody] TrasladarPacienteCommand command)
        {
            var result = await _mediator.Send(command);

            // Emitir la notificación de actualización del estado de las camas
            await _hubContext.Clients.All.SendAsync("ReceiveCamaUpdate", new
            {
                VersionEstado = GlobalStateVersion.Increment()
            });

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAreaClinicaCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
