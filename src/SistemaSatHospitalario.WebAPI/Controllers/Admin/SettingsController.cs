using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admin;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admin;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public SettingsController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        // --- GENERAL CONFIG ---
        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            var result = await _mediator.Send(new GetConfiguracionQuery());
            return Ok(result);
        }

        [HttpPost("config")]
        public async Task<IActionResult> UpdateConfig([FromBody] UpdateConfiguracionCommand command)
        {
            var result = await _mediator.Send(command);
            return result ? Ok() : BadRequest();
        }

        // --- SECURITY & RBAC ---
        [HttpGet("security")]
        public async Task<IActionResult> GetSecurityMatrix()
        {
            var result = await _mediator.Send(new GetSecurityConfigQuery());
            return Ok(result);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _identityService.CreateRoleAsync(roleName);
            return result ? Ok() : BadRequest(new { error = "El rol ya existe o no se pudo crear." });
        }

        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var result = await _identityService.DeleteRoleAsync(roleName);
            return result ? Ok() : BadRequest(new { error = "No se pudo eliminar el rol." });
        }

        [HttpPost("permissions")]
        public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionsCommand command)
        {
            var result = await _mediator.Send(command);
            return result ? Ok() : BadRequest();
        }

        // --- PHYSICIAN SCHEDULES ---
        [HttpGet("medicos/horarios")]
        public async Task<IActionResult> GetMedicosHorarios()
        {
            var result = await _mediator.Send(new GetMedicosHorariosQuery());
            return Ok(result);
        }

        [HttpPost("medicos/horarios/sync")]
        public async Task<IActionResult> SyncMedicoSchedules([FromBody] SyncMedicoSchedulesCommand command)
        {
            var result = await _mediator.Send(command);
            return result ? Ok() : BadRequest();
        }

        // --- USERS MANAGEMENT ---
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _identityService.GetUsersAsync());
        }

        [HttpGet("roles-list")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _identityService.GetRolesAsync());
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _identityService.CreateUserAsync(request.Username, request.Email, request.Password, request.Roles);
            return result ? Ok() : BadRequest(new { error = "Error al crear usuario." });
        }

        [HttpPost("users/permissions")]
        public async Task<IActionResult> UpdateUserPermissions([FromBody] UpdateUserPermissionsRequest request)
        {
            var result = await _identityService.UpdateUserPermissionsAsync(request.UserId, request.Permissions);
            return result ? Ok() : BadRequest();
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UpdateUserPermissionsRequest
    {
        public Guid UserId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
