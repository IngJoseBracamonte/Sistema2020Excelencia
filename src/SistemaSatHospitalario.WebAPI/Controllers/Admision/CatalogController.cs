using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("unified")]
        public async Task<ActionResult<List<CatalogItemDto>>> GetUnifiedCatalog([FromQuery] int? convenioId)
        {
            var query = new GetUnifiedCatalogQuery { ConvenioId = convenioId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateCatalogItemCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCatalogItemCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
