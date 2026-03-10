using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize] // Asegura que solo usuarios autenticados usen el sistema de Cajas.
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CajaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Abre la Caja Diaria Administrativa Principal para comenzar la recaudación global.
        /// </summary>
        [HttpPost("Abrir")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaCommand command)
        {
            try
            {
                var idCaja = await _mediator.Send(command);
                return Ok(new { Message = "Caja Diaria abierta con éxito.", CajaId = idCaja });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Clausura la Caja Diaria Principal, parando cualquier turno restante.
        /// </summary>
        [HttpPost("Cerrar")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CerrarCaja()
        {
            try
            {
                var result = await _mediator.Send(new CerrarCajaCommand());
                return Ok(new { Message = "La Caja Diaria fue clausurada con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }



        /// <summary>
        /// Obtiene un resumen en tiempo real de la recaudación de la caja matriz y el desglose de sus turnos.
        /// </summary>
        [HttpGet("Resumen")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerResumen()
        {
            try
            {
                var resumen = await _mediator.Send(new ObtenerResumenCajasQuery());
                return Ok(resumen);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
