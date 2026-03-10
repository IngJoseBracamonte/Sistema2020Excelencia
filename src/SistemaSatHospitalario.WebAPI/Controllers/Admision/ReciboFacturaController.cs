using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = "Administrador,Cajero")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReciboFacturaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReciboFacturaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cobra a un paciente mediante el soporte nativo de transacciones multidivisa.
        /// </summary>
        [HttpPost("RegistrarPagoMultidivisa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // En producción los internal server err loggean al OpenTelemetry.
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }
    }
}
