using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConveniosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConveniosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mediator.Send(new GetConveniosQuery()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConvenioCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateConvenioCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteConvenioCommand { Id = id }));
        }

        [HttpGet("{id}/precios")]
        public async Task<IActionResult> GetPrecios(int id)
        {
            return Ok(await _mediator.Send(new GetPreciosPorConvenioQuery(id)));
        }

        [HttpPost("precios")]
        public async Task<IActionResult> UpdatePrecio([FromBody] UpdateConvenioPerfilPrecioCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
