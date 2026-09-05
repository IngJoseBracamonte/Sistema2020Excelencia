using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoInterSedeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidoInterSedeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePedidoInterSedeDto dto)
        {
            var usuario = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "Sistema";
            var command = new CreatePedidoInterSedeCommand
            {
                Dto = dto,
                Usuario = usuario
            };
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            var result = await _mediator.Send(new GetPedidosInterSedePendientesQuery());
            return Ok(result);
        }

        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial()
        {
            var result = await _mediator.Send(new GetPedidosInterSedeHistorialQuery());
            return Ok(result);
        }

        [HttpPut("{id}/despachar")]
        public async Task<IActionResult> Dispatch(Guid id, [FromBody] DespacharPedidoDto? dto = null)
        {
            var usuario = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "Sistema";
            await _mediator.Send(new DispatchPedidoInterSedeCommand
            {
                PedidoId = id,
                Usuario = usuario,
                CantidadesAprobadas = dto?.CantidadesAprobadas,
                ObservacionesPorDetalle = dto?.ObservacionesPorDetalle
            });
            return NoContent();
        }

        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RechazarPedidoDto dto)
        {
            var usuario = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "Sistema";
            await _mediator.Send(new RechazarPedidoInterSedeCommand
            {
                PedidoId = id,
                Usuario = usuario,
                Motivo = dto?.Motivo ?? "Sin motivo especificado"
            });
            return NoContent();
        }

        [HttpPut("{id}/recibir")]
        public async Task<IActionResult> Receive(Guid id, [FromBody] global::System.Collections.Generic.Dictionary<Guid, decimal> discrepancias)
        {
            var usuario = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name ?? "Sistema";
            await _mediator.Send(new ReceivePedidoInterSedeCommand
            {
                PedidoId = id,
                Usuario = usuario,
                Discrepancias = discrepancias ?? new global::System.Collections.Generic.Dictionary<Guid, decimal>()
            });
            return NoContent();
        }
    }
}
