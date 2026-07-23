using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Infrastructure.Hubs;
using SistemaSatHospitalario.WebAPI.Infrastructure.Security;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.Medico + "," + AuthorizationConstants.AsistenteHospitalario + "," + AuthorizationConstants.AsistenteEmergencia + "," + AuthorizationConstants.Cajero + "," + AuthorizationConstants.Supervisor)]
    [ApiController]
    [Route("api/[controller]")]
    public class EnfermeriaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<DashboardHub> _hubContext;

        public EnfermeriaController(IMediator mediator, IHubContext<DashboardHub> hubContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _hubContext = hubContext;
        }

        [HttpPost("Triage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarTriage([FromBody] RegistrarTriageYValoracionCommand command)
        {
            try
            {
                command.UsuarioRegistro = User.GetUserName();
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("Triage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModificarTriage([FromBody] ModificarTriageYValoracionCommand command)
        {
            try
            {
                command.UsuarioRegistro = User.GetUserName();
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("TriageHistorial/{cuentaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTriageHistorial(Guid cuentaId)
        {
            try
            {
                var query = new GetTriageYValoracionHistoryQuery { CuentaServicioId = cuentaId };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("Traslado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TrasladarPaciente([FromBody] TrasladarPacienteCommand command)
        {
            try
            {
                command.UsuarioTraslado = User.GetUserName();
                var result = await _mediator.Send(command);

                // Difundir la actualización de camas con la nueva versión del estado
                await _hubContext.Clients.All.SendAsync("ReceiveCamaUpdate", new
                {
                    VersionEstado = GlobalStateVersion.Increment()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("CambioCama")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarCambioCama([FromBody] RegistrarCambioCamaCommand command)
        {
            try
            {
                command.UsuarioCarga = User.GetUserName();
                var result = await _mediator.Send(command);

                await _hubContext.Clients.All.SendAsync("ReceiveCamaUpdate", new
                {
                    VersionEstado = GlobalStateVersion.Increment()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("TrasladoArea")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarTrasladoArea([FromBody] RegistrarTrasladoAreaCommand command)
        {
            try
            {
                command.UsuarioTraslado = User.GetUserName();
                var result = await _mediator.Send(command);

                await _hubContext.Clients.All.SendAsync("ReceiveCamaUpdate", new
                {
                    VersionEstado = GlobalStateVersion.Increment()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
