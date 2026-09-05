using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SedeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetSedesQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSedeCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSedeCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSedeCommand { Id = id });
            return NoContent();
        }
    }
}
