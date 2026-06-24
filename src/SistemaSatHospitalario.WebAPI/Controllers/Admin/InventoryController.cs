using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public InventoryController(IApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        [HttpGet("insumos")]
        public async Task<IActionResult> GetInsumos(CancellationToken ct)
        {
            var insumos = await _context.Insumos
                .OrderBy(i => i.Nombre)
                .ToListAsync(ct);
            return Ok(insumos);
        }

        [HttpGet("stock-por-sede")]
        public async Task<IActionResult> GetStockPorSede([FromQuery] Guid sedeId, CancellationToken ct)
        {
            var stocks = await _context.StocksSedes
                .Include(s => s.Insumo)
                .Where(s => s.SedeId == sedeId)
                .Select(s => new
                {
                    s.InsumoId,
                    InsumoCodigo = s.Insumo.Codigo,
                    InsumoNombre = s.Insumo.Nombre,
                    s.StockActual,
                    s.StockMinimo,
                    s.StockMaximo
                })
                .ToListAsync(ct);
            return Ok(stocks);
        }

        [HttpPost("insumos")]
        public async Task<IActionResult> CreateInsumo([FromBody] CreateInsumoDto dto, CancellationToken ct)
        {
            if (await _context.Insumos.AnyAsync(i => i.Codigo == dto.Codigo, ct))
            {
                return BadRequest(new { Message = $"Ya existe un insumo con el código {dto.Codigo}." });
            }

            var insumo = new Insumo(dto.Codigo, dto.Nombre, dto.StockInicial, dto.UnidadMedidaBase, dto.CostoUnitarioBaseUSD);
            _context.Insumos.Add(insumo);

            if (dto.StockInicial != 0)
            {
                var principalSede = await _context.Sedes.FirstOrDefaultAsync(s => s.EsPrincipal && s.Activo, ct);
                var targetSedeId = principalSede?.Id ?? Guid.Empty;

                var stockSede = new StockSede(insumo.Id, targetSedeId, dto.StockInicial);
                _context.StocksSedes.Add(stockSede);

                var mov = new MovimientoInsumo(
                    insumo.Id,
                    targetSedeId,
                    "Ingreso",
                    dto.StockInicial,
                    dto.UnidadMedidaBase,
                    dto.StockInicial,
                    User.Identity?.Name ?? "System",
                    "Stock inicial de creación de insumo"
                );
                _context.MovimientosInsumo.Add(mov);
            }

            await _context.SaveChangesAsync(ct);
            return Ok(insumo);
        }

        [HttpPut("insumos/{id}")]
        public async Task<IActionResult> UpdateInsumo(Guid id, [FromBody] UpdateInsumoDto dto, CancellationToken ct)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == id, ct);
            if (insumo == null) return NotFound();

            insumo.ActualizarDetalles(dto.Nombre, dto.CostoUnitarioBaseUSD);
            await _context.SaveChangesAsync(ct);
            return Ok(insumo);
        }

        [HttpPost("movimientos")]
        public async Task<IActionResult> RecordMovement([FromBody] RecordMovementDto dto, CancellationToken ct)
        {
            var username = User.Identity?.Name ?? dto.Usuario ?? "System";
            await _inventoryService.RecordMovementAsync(
                dto.InsumoId,
                dto.SedeId,
                dto.TipoMovimiento,
                dto.CantidadOriginal,
                dto.UnidadMedidaOriginal,
                username,
                dto.Motivo,
                ct
            );
            return Ok(new { Success = true });
        }

        [HttpGet("movimientos")]
        public async Task<IActionResult> GetMovements(CancellationToken ct)
        {
            var movements = await _context.MovimientosInsumo
                .Include(m => m.Insumo)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync(ct);
            return Ok(movements);
        }

        [HttpPost("cierre")]
        public async Task<IActionResult> PerformClosing([FromBody] PerformClosingDto dto, CancellationToken ct)
        {
            var username = User.Identity?.Name ?? dto.Usuario ?? "System";
            await _inventoryService.PerformClosingAsync(
                dto.SedeId,
                username,
                dto.Observaciones,
                dto.Detalles,
                ct
            );
            return Ok(new { Success = true });
        }

        [HttpGet("recetas")]
        public async Task<IActionResult> GetRecetas(CancellationToken ct)
        {
            var recetas = await _context.ServiciosInsumoRecetas
                .Include(r => r.ServicioClinico)
                .Include(r => r.Insumo)
                .OrderBy(r => r.ServicioClinico.Descripcion)
                .ToListAsync(ct);
            return Ok(recetas);
        }

        [HttpPost("recetas")]
        public async Task<IActionResult> CreateOrUpdateRecipe([FromBody] CreateRecipeDto dto, CancellationToken ct)
        {
            var service = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == dto.ServicioClinicoId, ct);
            if (service == null) return BadRequest(new { Message = "El servicio clínico seleccionado no existe." });

            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == dto.InsumoId, ct);
            if (insumo == null) return BadRequest(new { Message = "El insumo seleccionado no existe." });

            var existing = await _context.ServiciosInsumoRecetas
                .FirstOrDefaultAsync(r => r.ServicioClinicoId == dto.ServicioClinicoId && r.InsumoId == dto.InsumoId, ct);

            if (existing != null)
            {
                existing.ActualizarReceta(dto.Cantidad, dto.UnidadMedidaConsumo);
            }
            else
            {
                var receta = new ServicioInsumoReceta(dto.ServicioClinicoId, service.Codigo, dto.InsumoId, dto.Cantidad, dto.UnidadMedidaConsumo);
                _context.ServiciosInsumoRecetas.Add(receta);
            }

            await _context.SaveChangesAsync(ct);
            return Ok(new { Success = true });
        }

        [HttpDelete("recetas/{id}")]
        public async Task<IActionResult> DeleteRecipe(Guid id, CancellationToken ct)
        {
            var recipe = await _context.ServiciosInsumoRecetas.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (recipe == null) return NotFound();

            _context.ServiciosInsumoRecetas.Remove(recipe);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }
    }

    public class CreateInsumoDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal StockInicial { get; set; }
        public UnidadMedida UnidadMedidaBase { get; set; }
        public decimal CostoUnitarioBaseUSD { get; set; }
    }

    public class UpdateInsumoDto
    {
        public string Nombre { get; set; }
        public decimal CostoUnitarioBaseUSD { get; set; }
    }

    public class RecordMovementDto
    {
        public Guid InsumoId { get; set; }
        public Guid SedeId { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal CantidadOriginal { get; set; }
        public UnidadMedida UnidadMedidaOriginal { get; set; }
        public string? Usuario { get; set; }
        public string Motivo { get; set; }
    }

    public class PerformClosingDto
    {
        public Guid SedeId { get; set; }
        public string? Usuario { get; set; }
        public string Observaciones { get; set; }
        public List<CierreDetalleInputDto> Detalles { get; set; }
    }

    public class CreateRecipeDto
    {
        public Guid ServicioClinicoId { get; set; }
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public UnidadMedida UnidadMedidaConsumo { get; set; }
    }
}
