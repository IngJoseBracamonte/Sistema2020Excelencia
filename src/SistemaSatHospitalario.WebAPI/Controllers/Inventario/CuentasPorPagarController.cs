using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Inventario;
using SistemaSatHospitalario.Core.Application.Queries.Inventario;
using SistemaSatHospitalario.Core.Application.DTOs.Inventario;

namespace SistemaSatHospitalario.WebAPI.Controllers.Inventario
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasPorPagarController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CuentasPorPagarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ordenes")]
        public async Task<ActionResult<List<OrdenCompraInventarioDto>>> GetOrdenes(
            [FromQuery] string? estado,
            [FromQuery] string? busqueda,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            var query = new GetOrdenesCompraQuery
            {
                Estado = estado,
                Busqueda = busqueda,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("registrar-pago")]
        public async Task<ActionResult<PagoProveedorDto>> RegistrarPago([FromBody] RegistrarPagoProveedorRequest request)
        {
            if (request == null) return BadRequest("La solicitud de pago es requerida.");

            var command = new RegistrarPagoProveedorCommand
            {
                OrdenCompraId = request.OrdenCompraId,
                MontoAbonadoUSD = request.MontoAbonadoUSD,
                TasaCambio = request.TasaCambio,
                MetodoPago = request.MetodoPago,
                Referencia = request.Referencia,
                Observaciones = request.Observaciones
            };

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("historial-pagos")]
        public async Task<ActionResult<List<PagoProveedorDto>>> GetHistorialPagos(
            [FromQuery] string? busqueda,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            var query = new GetHistorialPagosQuery
            {
                Busqueda = busqueda,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
