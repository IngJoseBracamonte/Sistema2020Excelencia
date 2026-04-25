using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admin;
using SistemaSatHospitalario.Core.Application.Queries;

using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.Cajero + "," + AuthorizationConstants.Supervisor + "," + AuthorizationConstants.AsistenteDeSeguros + "," + AuthorizationConstants.Medico)]
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicoDto>>> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllMedicosQuery()));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMedicoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateMedicoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteMedicoCommand { Id = id });
            return Ok(result);
        }

        [HttpGet("reporte/honorarios")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<ActionResult<List<DoctorHonorariaDto>>> GetHonorariaReport()
        {
            return Ok(await _mediator.Send(new GetDoctorHonorariaReportQuery()));
        }

        [HttpGet("reporte/calculo-honorarios")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<ActionResult<List<DoctorHonorariumSummaryDto>>> GetHonorariumSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var query = new GetDoctorHonorariumSummaryQuery { StartDate = startDate, EndDate = endDate };
            return Ok(await _mediator.Send(query));
        }
    }
}
