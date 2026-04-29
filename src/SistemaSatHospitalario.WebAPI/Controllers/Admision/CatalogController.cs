using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using Microsoft.Extensions.Logging;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IMediator mediator, ILogger<CatalogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("unified")]
        public async Task<ActionResult<List<CatalogItemDto>>> GetUnifiedCatalog([FromQuery] int? convenioId)
        {
            var query = new GetUnifiedCatalogQuery { ConvenioId = convenioId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("payment-methods")]
        public async Task<ActionResult<List<PaymentMethodDto>>> GetPaymentMethods()
        {
            var result = await _mediator.Send(new GetPaymentMethodsQuery());
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            _logger.LogWarning("[CATALOG-API] ATTEMPTING TO DELETE ITEM ID: {Id}", id);
            
            var result = await _mediator.Send(new DeleteCatalogItemCommand { Id = id });
            
            _logger.LogWarning("[CATALOG-API] DELETE RESULT FOR {Id}: {Result}", id, result);
            
            if (!result) return NotFound(new { message = "El servicio no existe o el ID es inválido" });
            
            return Ok(result);
        }
    }
}
