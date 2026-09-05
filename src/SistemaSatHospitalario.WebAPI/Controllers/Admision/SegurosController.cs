using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System.Linq;
using System.Collections.Generic;

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

            var cxcIds = dto.CuentasPorCobrarIds ?? new List<Guid>();
            if (dto.CuentaPorCobrarId.HasValue && !cxcIds.Contains(dto.CuentaPorCobrarId.Value))
            {
                cxcIds.Add(dto.CuentaPorCobrarId.Value);
            }

            foreach (var id in cxcIds)
            {
                var cxc = await _context.CuentasPorCobrar.FindAsync(id);
                if (cxc != null)
                {
                    cxc.MarcarCompromisoGenerado();
                    if (dto.AnexarGarantia)
                    {
                        cxc.MarcarGarantiaGenerada();
                    }

                    // [V12.8] Persistir ítems de garantía prendaria
                    await PersistirGarantiasItemsAsync(id, dto.GarantiasItems);
                    
                    // Registrar Auditoría
                    var log = new DocumentLog(
                        "Compromiso de Pago", 
                        cxc.Id.ToString(), 
                        "Generación", 
                        _currentUserService.UserId?.ToString() ?? "System", 
                        _currentUserService.UserName ?? "Sistema",
                        $"Generado para {dto.NombrePaciente}");
                    
                    _context.DocumentLogs.Add(log);
                }
            }

            if (cxcIds.Count > 0)
            {
                await _context.SaveChangesAsync(default);
            }

            // [V12.8] Si hay ítems, calcular MontoGarantia agregado para el PDF
            SincronizarMontoGarantiaDesdeItems(dto);

            var pdfBytes = _pdfService.GenerarCompromisoPagoPdf(dto, logoBase64);
            
            return File(pdfBytes, "application/pdf", $"Compromiso_Pago_{dto.CedulaResponsable}.pdf");
        }

        [HttpPost("conformidad-servicios")]
        public async Task<IActionResult> GenerarConformidadServicios([FromBody] CompromisoPagoDto dto)
        {
            var config = await _context.ConfiguracionGeneral.FirstOrDefaultAsync();
            var logoBase64 = config?.LogoBase64;

            var cxcIds = dto.CuentasPorCobrarIds ?? new List<Guid>();
            if (dto.CuentaPorCobrarId.HasValue && !cxcIds.Contains(dto.CuentaPorCobrarId.Value))
            {
                cxcIds.Add(dto.CuentaPorCobrarId.Value);
            }

            foreach (var id in cxcIds)
            {
                var cxc = await _context.CuentasPorCobrar.FindAsync(id);
                if (cxc != null)
                {
                    cxc.MarcarCompromisoGenerado();

                    // Buscar recibo de factura asociado
                    var recibo = await _context.RecibosFactura
                        .FirstOrDefaultAsync(r => r.CuentaServicioId == cxc.CuentaServicioId);
                    if (recibo != null && string.IsNullOrEmpty(dto.NroFactura))
                    {
                        dto.NroFactura = recibo.NumeroRecibo;
                    }

                    // Registrar Auditoría
                    var log = new DocumentLog(
                        "Conformidad de Servicios", 
                        cxc.Id.ToString(), 
                        "Generación", 
                        _currentUserService.UserId?.ToString() ?? "System", 
                        _currentUserService.UserName ?? "Sistema",
                        $"Conformidad generada para {dto.NombrePaciente}");
                    
                    _context.DocumentLogs.Add(log);
                }
            }

            if (cxcIds.Count > 0)
            {
                await _context.SaveChangesAsync(default);
            }

            var pdfBytes = _pdfService.GenerarConformidadServiciosPdf(dto, logoBase64);
            
            return File(pdfBytes, "application/pdf", $"Conformidad_{dto.CedulaPaciente}.pdf");
        }

        [HttpPost("garantia-pago")]
        public async Task<IActionResult> GenerarGarantia([FromBody] CompromisoPagoDto dto)
        {
            var config = await _context.ConfiguracionGeneral.FirstOrDefaultAsync();
            var logoBase64 = config?.LogoBase64;

            var cxcIds = dto.CuentasPorCobrarIds ?? new List<Guid>();
            if (dto.CuentaPorCobrarId.HasValue && !cxcIds.Contains(dto.CuentaPorCobrarId.Value))
            {
                cxcIds.Add(dto.CuentaPorCobrarId.Value);
            }

            foreach (var id in cxcIds)
            {
                var cxc = await _context.CuentasPorCobrar.FindAsync(id);
                if (cxc != null)
                {
                    cxc.MarcarGarantiaGenerada();
                    // [V12.8] Persistir ítems de garantía prendaria
                    await PersistirGarantiasItemsAsync(id, dto.GarantiasItems);

                    var log = new DocumentLog(
                        "Garantía de Pago", 
                        id.ToString(), 
                        "Generación", 
                        _currentUserService.UserId?.ToString() ?? "System", 
                        _currentUserService.UserName ?? "Sistema",
                        $"Garantía para {dto.NombrePaciente}");
                    
                    _context.DocumentLogs.Add(log);
                }
            }

            if (cxcIds.Count > 0)
            {
                await _context.SaveChangesAsync(default);
            }

            // [V12.8] Sincronizar monto total desde ítems
            SincronizarMontoGarantiaDesdeItems(dto);

            var pdfBytes = _pdfService.GenerarGarantiaPdf(dto, logoBase64);
            
            return File(pdfBytes, "application/pdf", $"Garantia_{dto.CedulaResponsable}.pdf");
        }

        /// <summary>
        /// [V12.8] Obtener ítems de garantía guardados para una CuentaPorCobrar (para reimpresión fiel).
        /// </summary>
        [HttpGet("garantias-items/{cuentaPorCobrarId}")]
        public async Task<IActionResult> GetGarantiasItems(Guid cuentaPorCobrarId)
        {
            var items = await _context.GarantiasItems
                .AsNoTracking()
                .Where(g => g.CuentaPorCobrarId == cuentaPorCobrarId)
                .OrderBy(g => g.FechaRegistro)
                .Select(g => new GarantiaItemDto
                {
                    Descripcion = g.Descripcion,
                    ValorEstimado = g.ValorEstimado
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("garantias-items/{cuentaPorCobrarId}")]
        public async Task<IActionResult> GuardarGarantiasItems(Guid cuentaPorCobrarId, [FromBody] List<GarantiaItemDto> items)
        {
            var cxc = await _context.CuentasPorCobrar.FindAsync(cuentaPorCobrarId);
            if (cxc != null)
            {
                cxc.MarcarGarantiaGenerada();
                await PersistirGarantiasItemsAsync(cuentaPorCobrarId, items);

                var log = new DocumentLog(
                    "Garantía de Pago (Guardar)", 
                    cuentaPorCobrarId.ToString(), 
                    "Actualización", 
                    _currentUserService.UserId?.ToString() ?? "System", 
                    _currentUserService.UserName ?? "Sistema",
                    $"Guardada lista de garantías para CuentaPorCobrar {cuentaPorCobrarId}");
                
                _context.DocumentLogs.Add(log);
                await _context.SaveChangesAsync(default);

                return Ok(new { Message = "Garantías guardadas exitosamente." });
            }
            return NotFound(new { Error = "Cuenta por cobrar no encontrada." });
        }


        [HttpGet("ingresados")]
        public async Task<IActionResult> GetPacientesIngresados(
            [FromQuery] DateTime? desde, 
            [FromQuery] DateTime? hasta, 
            [FromQuery] string? nombre,
            [FromQuery] string? estado)
        {
            var query = _context.CuentasPorCobrar
                .Include(c => c.Cuenta)
                .ThenInclude(c => c.Paciente)
                .Include(c => c.Cuenta)
                .ThenInclude(c => c.Detalles)
                .Where(c => c.GarantiaGenerada == true);

            if (desde.HasValue)
            {
                var desdeDate = desde.Value.Date;
                query = query.Where(c => c.FechaCreacion >= desdeDate);
            }
            
            if (hasta.HasValue)
            {
                var hastaDate = hasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.FechaCreacion <= hastaDate);
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                var upperNombre = nombre.ToUpper();
                query = query.Where(c => c.Cuenta.Paciente.NombreCorto.ToUpper().Contains(upperNombre) || 
                                         c.Cuenta.Paciente.CedulaPasaporte.ToUpper().Contains(upperNombre));
            }

            if (!string.IsNullOrEmpty(estado) && !estado.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                if (estado.Equals("CuentasPorCobrar", StringComparison.OrdinalIgnoreCase) || estado.Equals("Cuentas por Cobrar", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Estado != EstadoConstants.Cobrada && c.Estado != EstadoConstants.Pagada);
                }
                else if (estado.Equals("Pagada", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(c => c.Estado == EstadoConstants.Cobrada || c.Estado == EstadoConstants.Pagada);
                }
            }

            var cuentasData = await query
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
                    c.Estado,
                    EsMoroso = c.FechaCreacion < DateTime.UtcNow.AddDays(-30),
                    ConceptosList = c.Cuenta.Detalles.Select(d => d.Descripcion).ToList(),
                    GarantiaItemsList = c.GarantiasItems.Select(gi => gi.Descripcion).ToList()
                })
                .ToListAsync();

            var cuentas = cuentasData.Select(c => new
            {
                c.Id,
                c.FechaCreacion,
                c.PacienteNombre,
                c.PacienteCedula,
                c.PacienteId,
                c.MontoTotalBase,
                c.SeguroNombre,
                c.CompromisoGenerado,
                c.GarantiaGenerada,
                c.Estado,
                c.EsMoroso,
                Conceptos = string.Join(", ", c.ConceptosList),
                GarantiaDescripcion = string.Join(", ", c.GarantiaItemsList)
            }).ToList();

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

        #region [V12.8] Helpers de Garantía Prendaria

        /// <summary>
        /// Persiste los ítems de garantía en la base de datos, reemplazando los existentes (idempotente para reimpresión).
        /// </summary>
        private async Task PersistirGarantiasItemsAsync(Guid cuentaPorCobrarId, List<GarantiaItemDto> items)
        {
            if (items == null || items.Count == 0) return;

            // Eliminar ítems previos para esta CxC (soporte de reimpresión/actualización)
            var existentes = await _context.GarantiasItems
                .Where(g => g.CuentaPorCobrarId == cuentaPorCobrarId)
                .ToListAsync();

            if (existentes.Any())
            {
                _context.GarantiasItems.RemoveRange(existentes);
            }

            // Crear nuevos ítems
            foreach (var item in items)
            {
                var entity = new GarantiaItem(cuentaPorCobrarId, item.Descripcion, item.ValorEstimado);
                _context.GarantiasItems.Add(entity);
            }
        }

        /// <summary>
        /// Si hay ítems individuales, calcula MontoGarantia y DescripcionGarantia agregados para retrocompatibilidad del PDF.
        /// </summary>
        private static void SincronizarMontoGarantiaDesdeItems(CompromisoPagoDto dto)
        {
            if (dto.GarantiasItems != null && dto.GarantiasItems.Count > 0)
            {
                dto.MontoGarantia = dto.GarantiasItems.Sum(i => i.ValorEstimado);
                dto.DescripcionGarantia = string.Join(", ", dto.GarantiasItems.Select(i => i.Descripcion));
            }
        }

        #endregion
    }
}
