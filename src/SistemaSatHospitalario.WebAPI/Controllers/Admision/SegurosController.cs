using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System.Linq;

using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.AsistenteSeguro + "," + AuthorizationConstants.AsistenteDeSeguros)]
    [ApiController]
    [Route("api/[controller]")]
    public class SegurosController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public SegurosController(IPdfService pdfService, IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _pdfService = pdfService;
            _context = context;
            _currentUserService = currentUserService;
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
                    
                    // Registrar Auditoría
                    var log = new DocumentLog(
                        "Compromiso de Pago", 
                        cxc.Id.ToString(), 
                        "Generación", 
                        _currentUserService.UserId?.ToString() ?? "System", 
                        _currentUserService.UserName ?? "Sistema",
                        $"Generado para {dto.NombrePaciente}");
                    
                    _context.DocumentLogs.Add(log);
                    
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

            // Actualizar estado en DB si el ID está presente
            if (dto.CuentaPorCobrarId.HasValue)
            {
                var cxc = await _context.CuentasPorCobrar.FindAsync(dto.CuentaPorCobrarId.Value);
                if (cxc != null)
                {
                    cxc.MarcarGarantiaGenerada();
                }
            }

            // Registrar Auditoría

            var log = new DocumentLog(
                "Garantía de Pago", 
                dto.CuentaPorCobrarId?.ToString() ?? "N/A", 
                "Generación", 
                _currentUserService.UserId?.ToString() ?? "System", 
                _currentUserService.UserName ?? "Sistema",
                $"Garantía para {dto.NombrePaciente}");
            
            _context.DocumentLogs.Add(log);
            await _context.SaveChangesAsync(default);

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
                    c.CompromisoGenerado,
                    c.GarantiaGenerada,
                    EsMoroso = c.FechaCreacion < DateTime.UtcNow.AddDays(-30)

                })
                .ToListAsync();

            return Ok(cuentas);
        }

        [HttpGet("consolidado-gerencia")]
        public async Task<IActionResult> GetConsolidadoGerencia()
        {
            var hoy = DateTime.UtcNow;
            var haceUnMes = hoy.AddDays(-30);

            var consolidado = await _context.CuentasPorCobrar
                .AsNoTracking()
                .Include(c => c.Cuenta)
                .ThenInclude(c => c.Convenio)
                .GroupBy(c => new { c.Cuenta.ConvenioId, c.Cuenta.Convenio.Nombre })
                .Select(g => new
                {
                    Nombre = g.Key.Nombre ?? "Convenio #" + g.Key.ConvenioId,
                    Pacientes = g.Count(),
                    Monto = g.Sum(c => c.MontoTotalBase - c.MontoPagadoBase),
                    Criticos = g.Count(c => c.FechaCreacion < haceUnMes && c.Estado != EstadoConstants.Cobrada)
                })
                .ToListAsync();

            return Ok(consolidado);
        }
    }
}

