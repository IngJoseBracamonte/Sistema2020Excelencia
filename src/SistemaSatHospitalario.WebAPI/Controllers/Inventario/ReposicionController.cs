using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.WebAPI.Infrastructure.Security;

namespace SistemaSatHospitalario.WebAPI.Controllers.Inventario
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.Supervisor)]
    [ApiController]
    [Route("api/Inventario/[controller]")]
    public class ReposicionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReposicionController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        private string CurrentUser => User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "Sistema";

        /// <summary>
        /// Procesa una reposición, devolución o cambio de talla/insumo entre sedes y sub-áreas sin desfase de stock.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcesarReposicion([FromBody] ProcesarReposicionStockCommand command)
        {
            try
            {
                command.UsuarioId = CurrentUser;
                var result = await _mediator.Send(command);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el historial de reposiciones y transferencias de insumos con filtros.
        /// </summary>
        [HttpGet("Historial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistorial([FromQuery] Guid? sedeId, [FromQuery] Guid? insumoId, [FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta, [FromQuery] string? motivo)
        {
            var result = await _mediator.Send(new GetReposicionesHistorialQuery
            {
                SedeId = sedeId,
                InsumoId = insumoId,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Motivo = motivo
            });
            return Ok(result);
        }
    }
}
