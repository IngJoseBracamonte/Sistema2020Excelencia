using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

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
        public async Task<IActionResult> GetDoctorSchedule(Guid doctorId, DateTime date)
        {
            var schedule = await _mediator.Send(new GetDoctorScheduleQuery(doctorId, date));
            return Ok(schedule);
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
