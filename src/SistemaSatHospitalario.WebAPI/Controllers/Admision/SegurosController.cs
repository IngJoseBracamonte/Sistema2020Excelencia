using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System.Linq;

using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.AsistenteSeguro + "," + AuthorizationConstants.AsistenteDeSeguros)]
    [ApiController]
    [Route("api/[controller]")]
    public class SegurosController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly IApplicationDbContext _context;

        public SegurosController(IPdfService pdfService, IApplicationDbContext context)
        {
            _pdfService = pdfService;
            _context = context;
        }

        [HttpPost("compromiso-pago")]
        public async Task<IActionResult> GenerarCompromisoPago([FromBody] CompromisoPagoDto dto)
        {
            var config = await _context.ConfiguracionGeneral.FirstOrDefaultAsync();
            var logoBase64 = config?.LogoBase64;

            if (dto.CuentaPorCobrarId.HasValue)
            {
                var cxc = await _context.CuentasPorCobrar.FindAsync(dto.CuentaPorCobrarId.Value);
                if (cxc != null)
                {
                    cxc.MarcarCompromisoGenerado();
                    await _context.SaveChangesAsync(default);
                }
            }

            var pdfBytes = _pdfService.GenerarCompromisoPagoPdf(dto, logoBase64);
            
            return File(pdfBytes, "application/pdf", $"Compromiso_Pago_{dto.CedulaResponsable}.pdf");
        }

        [HttpPost("garantia-pago")]
        public async Task<IActionResult> GenerarGarantia([FromBody] CompromisoPagoDto dto)
        {
            var config = await _context.ConfiguracionGeneral.FirstOrDefaultAsync();
            var logoBase64 = config?.LogoBase64;

            var pdfBytes = _pdfService.GenerarGarantiaPdf(dto, logoBase64);
            
            return File(pdfBytes, "application/pdf", $"Garantia_{dto.CedulaResponsable}.pdf");
        }

        [HttpGet("ingresados")]
        public async Task<IActionResult> GetPacientesIngresados(
            [FromQuery] DateTime? desde, 
            [FromQuery] DateTime? hasta, 
            [FromQuery] string? nombre,
            [FromQuery] bool? conCompromiso)
        {
            var query = _context.CuentasPorCobrar
                .Include(c => c.Cuenta)
                .ThenInclude(c => c.Paciente)
                .Where(c => c.Cuenta.TipoIngreso == "Seguro");

            if (desde.HasValue)
                query = query.Where(c => c.FechaCreacion.Date >= desde.Value.Date);
            
            if (hasta.HasValue)
                query = query.Where(c => c.FechaCreacion.Date <= hasta.Value.Date);

            if (!string.IsNullOrEmpty(nombre))
            {
                var upperNombre = nombre.ToUpper();
                query = query.Where(c => c.Cuenta.Paciente.NombreCorto.ToUpper().Contains(upperNombre) || 
                                         c.Cuenta.Paciente.CedulaPasaporte.Contains(nombre));
            }

            if (conCompromiso.HasValue)
                query = query.Where(c => c.CompromisoGenerado == conCompromiso.Value);

            var cuentas = await query
                .OrderByDescending(c => c.FechaCreacion)
                .Select(c => new
                {
                    c.Id,
                    c.FechaCreacion,
                    PacienteNombre = c.Cuenta.Paciente.NombreCompleto,
                    PacienteCedula = c.Cuenta.Paciente.CedulaPasaporte,
                    PacienteId = c.Cuenta.Paciente.Id,
                    c.MontoTotalBase,
                    SeguroNombre = "Convenio #" + c.Cuenta.ConvenioId,
                    c.CompromisoGenerado
                })
                .ToListAsync();

            return Ok(cuentas);
        }
    }
}
