using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.WebAPI.Infrastructure.Security;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.Medico + "," + AuthorizationConstants.AsistenteHospitalario + "," + AuthorizationConstants.AsistenteEmergencia + "," + AuthorizationConstants.Cajero + "," + AuthorizationConstants.Supervisor)]
    [ApiController]
    [Route("api/[controller]")]
    public class PabellonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PabellonController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Obtiene el listado de ordenes de cirugía filtradas por rango de fechas o estado.
        /// </summary>
        [HttpGet("Ordenes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdenes([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin, [FromQuery] string? estado)
        {
            var result = await _mediator.Send(new GetOrdenesCirugiaQuery
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Estado = estado
            });
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el detalle completo de una orden de cirugía, incluyendo sus logs de auditoría e insumos consumidos.
        /// </summary>
        [HttpGet("Ordenes/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdenDetalle(Guid id)
        {
            var result = await _mediator.Send(new GetOrdenCirugiaDetalleQuery { OrdenCirugiaId = id });
            if (result == null) return NotFound(new { message = "Orden de cirugía no encontrada." });
            return Ok(result);
        }

        /// <summary>
        /// Crea y agenda una nueva orden de cirugía.
        /// </summary>
        [HttpPost("Ordenes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearOrden([FromBody] CrearOrdenCirugiaCommand command)
        {
            try
            {
                command.UsuarioCreacion = User.GetUserName();
                var result = await _mediator.Send(command);
                return Ok(new { id = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambia el estado de una orden de cirugía (Iniciar, Completar, Cancelar).
        /// </summary>
        [HttpPost("Ordenes/Estado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoCirugiaCommand command)
        {
            try
            {
                command.UsuarioId = User.GetUserName();
                var result = await _mediator.Send(command);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Procesa la devolución masiva de insumos de cirugía retornándolos a la Sede Principal.
        /// </summary>
        [HttpPost("DevolucionMasiva")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DevolucionMasiva([FromBody] ProcesarDevolucionCirugiaMasivaCommand command)
        {
            try
            {
                command.UsuarioId = User.GetUserName();
                var result = await _mediator.Send(command);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Anexa un cargo extra (procedimiento, medicamento, insumo) a la cuenta durante una cirugía.
        /// </summary>
        [HttpPost("CargoExtra")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargoExtra([FromBody] AnexarCargoExtraCirugiaCommand command)
        {
            try
            {
                command.UsuarioId = User.GetUserName();
                var result = await _mediator.Send(command);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
