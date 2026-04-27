using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReciboFacturaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPdfService _pdfService;

        public ReciboFacturaController(IMediator mediator, IPdfService pdfService)
        {
            _mediator = mediator;
            _pdfService = pdfService;
        }

        [HttpPost("RegistrarPagoMultidivisa")]
        public async Task<IActionResult> RegistrarPago([FromBody] RegistrarReciboFacturaCommand command)
        {
            try
            {
                // Enriquecimiento de Auditoría (V15.0)
                command.CajeroUserId = User.Identity?.Name ?? "Sistama";
                
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

        [HttpPost("EmitirFactura")]
        [Authorize(Roles = "Admin,Administrador,Cajero")]
        public async Task<IActionResult> EmitirFactura([FromBody] EmitirFacturaFiscalCommand command)
        {
            try
            {
                command.UsuarioEmision = User.Identity?.Name ?? "Sistama";
                var result = await _mediator.Send(command);
                if (result) return Ok(new { Message = "Factura emitida formalmente." });
                return BadRequest(new { Error = "No se pudo emitir la factura." });
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

        [HttpGet("{id}/Download")]
        [AllowAnonymous] // Permitimos acceso directo para la pestaña de impresión (V12.4)
        public async Task<IActionResult> Download(Guid id)
        {
            // 1. Obtener datos serializados para el PDF
            var data = await _mediator.Send(new GetReciboPdfQuery { ReciboId = id });
            if (data == null) return NotFound("El recibo no existe.");
 
            // 2. Generar PDF vía QuestPDF
            var pdfBytes = _pdfService.GenerarReciboPdf(data);
 
            // 3. Retornar archivo para visualización en navegador
            return File(pdfBytes, "application/pdf", $"Recibo_{data.NumeroRecibo}.pdf");
        }

        [HttpPost("GeneratePdf")]
        public IActionResult GeneratePdf([FromBody] ReciboPdfDto data)
        {
            try
            {
                var pdfBytes = _pdfService.GenerarReciboPdf(data);
                return File(pdfBytes, "application/pdf", "Recibo_Prueba.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
