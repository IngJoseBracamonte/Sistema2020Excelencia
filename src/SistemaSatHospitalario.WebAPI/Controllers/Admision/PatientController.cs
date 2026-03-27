using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System;
using SistemaSatHospitalario.Core.Application.Commands.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<PatientDto>>> Search([FromQuery] string term)
        {
            var query = new SearchPatientQuery { SearchTerm = term };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<List<PatientHistoryDto>>> GetHistory(Guid id)
        {
            var query = new GetPatientHistoryQuery { PacienteId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
