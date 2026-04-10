using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/global-settings")]
    public class GlobalSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GlobalSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            return Ok(await _mediator.Send(new GetConfiguracionQuery()));
        }

        [HttpPost("config")]
        public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfiguracionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [AllowAnonymous]
        [HttpGet("tasa")]
        public async Task<IActionResult> GetTasa()
        {
            var tasa = await _mediator.Send(new GetTasaCambioQuery());
            return Ok(new { monto = tasa });
        }

        [HttpPost("tasa")]
        public async Task<IActionResult> UpdateTasa([FromBody] UpdateTasaCambioCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _mediator.Send(new GetUsersQuery()));
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _mediator.Send(new GetRolesQuery()));
        }

        [HttpPost("users/roles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
