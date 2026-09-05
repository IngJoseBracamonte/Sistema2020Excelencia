using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admin.HonorariumRules;
using SistemaSatHospitalario.Core.Application.Queries.Admin;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HonorariumRulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HonorariumRulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _mediator.Send(new GetMappingRulesQuery());
            return Ok(rules);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMappingRuleCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteMappingRuleCommand { Id = id });
            return NoContent();
        }
    }
}
