using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReciboFacturaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReciboFacturaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("RegistrarPagoMultidivisa")]
        public async Task<IActionResult> RegistrarPago([FromBody] RegistrarReciboFacturaCommand command)
        {
            try
            {
                var idRecibo = await _mediator.Send(command);
                return Ok(new 
                { 
                    Message = "El pago multidivisa ha sido asimilado, los saldos se registraron a la Orden y Caja en sesión.", 
                    ReciboId = idRecibo 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}/Print")]
        public async Task<ActionResult<ReciboPdfDto>> GetPrintData(Guid id)
        {
            var result = await _mediator.Send(new GetReciboPdfQuery { ReciboId = id });
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
