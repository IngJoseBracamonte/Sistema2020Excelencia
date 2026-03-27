using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppointmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Doctors/{specialty}")]
        public async Task<IActionResult> GetDoctorsBySpecialty(string specialty)
        {
            var doctors = await _mediator.Send(new GetDoctorsBySpecialtyQuery(specialty));
            return Ok(doctors);
        }

        [HttpGet("Schedule/{doctorId}/{date}")]
        public async Task<IActionResult> GetDoctorSchedule(Guid doctorId, DateTime date, [FromQuery] Guid? pacienteId = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                        ?? "Anonimo";

            var schedule = await _mediator.Send(new GetDoctorScheduleQuery(doctorId, date, userId, pacienteId));
            return Ok(schedule);
        }

        [HttpPost("Admin/Manage")]
        public async Task<IActionResult> AdminManageSchedule([FromBody] AdminManageScheduleCommand command)
        {
            // Verificación simple de rol para entorno de desarrollo/producción
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "admin" && role != "administrador")
            {
                return Forbid();
            }

            var result = await _mediator.Send(command);
            return result ? Ok(new { Success = true }) : NotFound(new { Success = false });
        }

        [HttpPost("Agendar")]
        public async Task<IActionResult> AgendarTurno([FromBody] AgendarTurnoCommand command)
        {
            try
            {
                var idTurno = await _mediator.Send(command);
                return Ok(new { Message = "Turno agendado con éxito.", TurnoId = idTurno });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
