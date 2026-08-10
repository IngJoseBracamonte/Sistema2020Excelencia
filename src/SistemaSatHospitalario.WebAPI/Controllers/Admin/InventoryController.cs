using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
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
    [Route("api/v1/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;
        private readonly IMediator _mediator;

        public InventoryController(IApplicationDbContext context, IInventoryService inventoryService, IMediator mediator)
        {
            _context = context;
            _inventoryService = inventoryService;
            _mediator = mediator;
        }

        [HttpGet("stock/by-code/{codigo}")]
        public async Task<IActionResult> GetStockByCodigo(string codigo, CancellationToken ct)
        {
            var insumo = await _context.Insumos
                .Include(i => i.StocksPorSede)
                .FirstOrDefaultAsync(i => i.Codigo == codigo && !i.IsDeleted, ct);

            if (insumo == null) return NotFound(new { Message = $"No se encontró insumo con código {codigo}." });

            var targetSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;
            var stockPrincipal = insumo.StocksPorSede
                .Where(s => s.SedeId == targetSedeId)
                .Select(s => (decimal?)s.StockActual)
                .FirstOrDefault() ?? insumo.StockActual;

            return Ok(new
            {
                insumo.Id,
                insumo.Codigo,
                insumo.Nombre,
                StockActual = stockPrincipal,
                UnidadMedidaBase = insumo.UnidadMedidaBase.ToString()
            });
        }

        [HttpGet("insumos")]
        public async Task<IActionResult> GetInsumos([FromQuery] bool? excludeHidden, [FromQuery] string? search, [FromQuery] Guid? sedeId, CancellationToken ct)
        {
            var targetSedeId = sedeId ?? SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;

            var query = _context.Insumos
                .Include(i => i.PrincipiosActivos)
                    .ThenInclude(pa => pa.PrincipioActivo)
                .Include(i => i.StocksPorSede)
                .AsQueryable();

            if (excludeHidden == true)
            {
                query = query.Where(i => !i.IsDeleted && !i.OcultoEnTraslados);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(i => 
                    i.Nombre.ToLower().Contains(searchLower) || 
                    i.Codigo.ToLower().Contains(searchLower) ||
                    i.PrincipiosActivos.Any(pa => pa.PrincipioActivo.Nombre.ToLower().Contains(searchLower))
                );
            }

            var insumos = await query
                .OrderBy(i => i.Nombre)
                .Select(i => new
                {
                    i.Id,
                    i.Codigo,
                    i.Nombre,
                    StockActual = i.StocksPorSede.Where(s => s.SedeId == targetSedeId).Select(s => (decimal?)s.StockActual).FirstOrDefault() ?? 0,
                    UnidadMedidaBase = i.UnidadMedidaBase.ToString(),
                    i.CostoUnitarioBaseUSD,
                    i.PermiteFraccionamiento,
                    i.Categoria,
                    i.IsDeleted,
                    i.FechaInactivacion,
                    i.OcultoEnTraslados,
                    PrincipiosActivos = i.PrincipiosActivos.Select(pa => new
                    {
                        pa.PrincipioActivoId,
                        Nombre = pa.PrincipioActivo.Nombre,
                        pa.Concentracion
                    })
                })
                .ToListAsync(ct);

            return Ok(insumos);
        }

        [HttpGet("insumos/{id}")]
        public async Task<IActionResult> GetInsumoById(Guid id, CancellationToken ct)
        {
            var insumo = await _context.Insumos
                .Include(i => i.PrincipiosActivos)
                    .ThenInclude(pa => pa.PrincipioActivo)
                .Include(i => i.StocksPorSede)
                .FirstOrDefaultAsync(i => i.Id == id, ct);

            if (insumo == null) return NotFound();

            var stockPrincipal = insumo.StocksPorSede
                .Where(s => s.SedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal)
                .Select(s => (decimal?)s.StockActual)
                .FirstOrDefault() ?? insumo.StocksPorSede.Select(s => (decimal?)s.StockActual).Sum() ?? 0;

            return Ok(new
            {
                insumo.Id,
                insumo.Codigo,
                insumo.Nombre,
                StockActual = stockPrincipal,
                UnidadMedidaBase = insumo.UnidadMedidaBase.ToString(),
                insumo.CostoUnitarioBaseUSD,
                insumo.PermiteFraccionamiento,
                insumo.Categoria,
                insumo.IsDeleted,
                insumo.FechaInactivacion,
                insumo.OcultoEnTraslados,
                PrincipiosActivos = insumo.PrincipiosActivos.Select(pa => new
                {
                    pa.PrincipioActivoId,
                    Nombre = pa.PrincipioActivo.Nombre,
                    pa.Concentracion
                })
            });
        }

        [HttpGet("stock-por-sede")]
        public async Task<IActionResult> GetStockPorSede([FromQuery] Guid sedeId, CancellationToken ct)
        {
            var stocks = await _context.StocksSedes
                .Include(s => s.Insumo)
                .Where(s => s.SedeId == sedeId && !s.Insumo.IsDeleted)
                .Select(s => new
                {
                    s.InsumoId,
                    InsumoCodigo = s.Insumo.Codigo,
                    InsumoNombre = s.Insumo.Nombre,
                    UnidadMedidaBase = s.Insumo.UnidadMedidaBase.ToString(),
                    s.StockActual,
                    s.StockMinimo,
                    s.StockMaximo
                })
                .OrderBy(s => s.InsumoNombre)
                .ToListAsync(ct);
            return Ok(stocks);
        }

        [HttpGet("stock-sede/{sedeId}")]
        public async Task<IActionResult> GetStockSedeById(Guid sedeId, CancellationToken ct)
        {
            var stocks = await _context.StocksSedes
                .Include(s => s.Insumo)
                .Where(s => s.SedeId == sedeId && !s.Insumo.IsDeleted)
                .Select(s => new
                {
                    InsumoId = s.InsumoId,
                    Codigo = s.Insumo.Codigo,
                    Nombre = s.Insumo.Nombre,
                    UnidadMedidaBase = s.Insumo.UnidadMedidaBase.ToString(),
                    StockActual = s.StockActual,
                    StockMinimo = s.StockMinimo,
                    StockMaximo = s.StockMaximo
                })
                .OrderBy(s => s.Nombre)
                .ToListAsync(ct);

            return Ok(stocks);
        }

        [HttpGet("stock-consolidado")]
        public async Task<IActionResult> GetStockConsolidado(CancellationToken ct)
        {
            var insumos = await _context.Insumos
                .Include(i => i.StocksPorSede)
                .Where(i => !i.IsDeleted)
                .Select(i => new
                {
                    InsumoId = i.Id,
                    Codigo = i.Codigo,
                    Nombre = i.Nombre,
                    UnidadMedidaBase = i.UnidadMedidaBase.ToString(),
                    StockActual = i.StocksPorSede.Any() ? i.StocksPorSede.Sum(s => s.StockActual) : i.StockActual,
                    StockMinimo = 5,
                    SedeNombre = "Consolidado Global"
                })
                .OrderBy(i => i.Nombre)
                .ToListAsync(ct);

            return Ok(insumos);
        }

        [HttpGet("kardex")]
        public async Task<IActionResult> GetKardex(
            [FromQuery] Guid? sedeId,
            [FromQuery] Guid? insumoId,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            CancellationToken ct)
        {
            var query = new GetKardexQuery
            {
                SedeId = sedeId,
                InsumoId = insumoId,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            };
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpPost("insumos")]
        public async Task<IActionResult> CreateInsumo([FromBody] CreateInsumoDto dto, CancellationToken ct)
        {
            if (await _context.Insumos.AnyAsync(i => i.Codigo == dto.Codigo, ct))
            {
                return BadRequest(new { Message = $"Ya existe un insumo con el código {dto.Codigo}." });
            }

            var insumo = new Insumo(dto.Codigo, dto.Nombre, dto.StockInicial, dto.UnidadMedidaBase, dto.CostoUnitarioBaseUSD, dto.PermiteFraccionamiento, dto.Categoria);
            _context.Insumos.Add(insumo);

            if (dto.StockInicial > 0)
            {
                var principalSede = await _context.Sedes.FirstOrDefaultAsync(s => s.EsPrincipal && s.Activo, ct);
                var targetSedeId = principalSede?.Id ?? SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;

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
            return Ok(new
            {
                insumo.Id,
                insumo.Codigo,
                insumo.Nombre,
                StockActual = insumo.StockActual,
                UnidadMedidaBase = insumo.UnidadMedidaBase.ToString(),
                insumo.CostoUnitarioBaseUSD,
                insumo.PermiteFraccionamiento,
                insumo.Categoria,
                insumo.IsDeleted,
                insumo.OcultoEnTraslados
            });
        }

        [HttpPut("insumos/{id}")]
        public async Task<IActionResult> UpdateInsumo(Guid id, [FromBody] UpdateInsumoDto dto, CancellationToken ct)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == id, ct);
            if (insumo == null) return NotFound();

            insumo.ActualizarDetalles(
                dto.Nombre, 
                dto.UnidadMedidaBase, 
                dto.CostoUnitarioBaseUSD, 
                dto.PermiteFraccionamiento, 
                dto.Categoria
            );
            await _context.SaveChangesAsync(ct);
            return Ok(new
            {
                insumo.Id,
                insumo.Codigo,
                insumo.Nombre,
                StockActual = insumo.StockActual,
                UnidadMedidaBase = insumo.UnidadMedidaBase.ToString(),
                insumo.CostoUnitarioBaseUSD,
                insumo.PermiteFraccionamiento,
                insumo.Categoria,
                insumo.IsDeleted,
                insumo.OcultoEnTraslados
            });
        }

        // --- Principios Activos CRUD & Vinculación ---

        [HttpGet("principios-activos")]
        public async Task<IActionResult> GetPrincipiosActivos(CancellationToken ct)
        {
            var pas = await _context.PrincipiosActivos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);
            return Ok(pas);
        }

        [HttpPost("principios-activos")]
        public async Task<IActionResult> CreatePrincipioActivo([FromBody] CreatePrincipioActivoDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest(new { Message = "El nombre del principio activo es obligatorio." });
            }

            var existing = await _context.PrincipiosActivos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == dto.Nombre.Trim().ToLower(), ct);

            if (existing != null)
            {
                return Ok(existing);
            }

            var pa = new PrincipioActivo(dto.Nombre);
            _context.PrincipiosActivos.Add(pa);
            await _context.SaveChangesAsync(ct);
            return Ok(pa);
        }

        [HttpPost("insumos/{id}/principios-activos")]
        public async Task<IActionResult> VincularPrincipioActivo(Guid id, [FromBody] VincularPrincipioActivoDto dto, CancellationToken ct)
        {
            var insumo = await _context.Insumos
                .Include(i => i.PrincipiosActivos)
                .FirstOrDefaultAsync(i => i.Id == id, ct);

            if (insumo == null) return NotFound(new { Message = "Insumo no encontrado." });

            var pa = await _context.PrincipiosActivos.FirstOrDefaultAsync(p => p.Id == dto.PrincipioActivoId, ct);
            if (pa == null) return BadRequest(new { Message = "Principio activo no encontrado." });

            try
            {
                insumo.AgregarPrincipioActivo(pa, dto.Concentracion);
                await _context.SaveChangesAsync(ct);
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("insumos/{id}/principios-activos/{paId}")]
        public async Task<IActionResult> DesvincularPrincipioActivo(Guid id, Guid paId, CancellationToken ct)
        {
            var insumo = await _context.Insumos
                .Include(i => i.PrincipiosActivos)
                .FirstOrDefaultAsync(i => i.Id == id, ct);

            if (insumo == null) return NotFound();

            insumo.RemoverPrincipioActivo(paId);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        // --- Operaciones Transaccionales puro ---

        [HttpPost("descarte")]
        public async Task<IActionResult> RegistrarDescarte([FromBody] RegistrarDescarteRequestDto dto, CancellationToken ct)
        {
            try
            {
                var command = new RegistrarDescarteCommand
                {
                    InsumoId = dto.InsumoId,
                    SedeId = dto.SedeId,
                    Cantidad = dto.Cantidad,
                    Motivo = dto.Motivo,
                    Usuario = User.Identity?.Name ?? "System"
                };

                await _mediator.Send(command, ct);
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("envio-subarea")]
        public async Task<IActionResult> EnviarASubArea([FromBody] EnviarASubAreaCommand command, CancellationToken ct)
        {
            try
            {
                command.Usuario = User.Identity?.Name ?? command.Usuario ?? "System";
                var result = await _mediator.Send(command, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("movimientos")]
        public async Task<IActionResult> GetMovements(
            [FromQuery] string? tipoMovimiento,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] string? search,
            CancellationToken ct)
        {
            var query = new GetHistorialMovimientosQuery
            {
                TipoMovimiento = tipoMovimiento,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                SearchTerm = search
            };
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpGet("motivos-descarte")]
        public IActionResult GetMotivosDescarte()
        {
            var motivos = new[]
            {
                new { Id = "10000000-0000-0000-0000-000000000101", Codigo = "VENC", Nombre = "Vencimiento / Caducidad" },
                new { Id = "10000000-0000-0000-0000-000000000102", Codigo = "DANO", Nombre = "Avería / Rotura / Daño Físico" },
                new { Id = "10000000-0000-0000-0000-000000000103", Codigo = "DETE", Nombre = "Deterioro / Contaminación" },
                new { Id = "10000000-0000-0000-0000-000000000104", Codigo = "AUDI", Nombre = "Ajuste de Auditoría" }
            };
            return Ok(motivos);
        }

        [HttpGet("tipos-movimiento")]
        public IActionResult GetTiposMovimiento()
        {
            var tipos = new[]
            {
                new { Id = "Ingreso", Codigo = "ING", Nombre = "Ingreso / Compra Insumo" },
                new { Id = "EnvioSubArea", Codigo = "ENV", Nombre = "Envío Directo a Sub-Área" },
                new { Id = "Descarte", Codigo = "DES", Nombre = "Descarte / Baja de Inventario" },
                new { Id = "Ajuste", Codigo = "AJU", Nombre = "Ajuste Kárdex Auditoría" }
            };
            return Ok(tipos);
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

        [HttpDelete("insumos/{id}")]
        public async Task<IActionResult> DeleteInsumo(Guid id, CancellationToken ct)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == id, ct);
            if (insumo == null) return NotFound();

            insumo.SoftDelete();
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpPost("insumos/{id}/restaurar")]
        public async Task<IActionResult> RestoreInsumo(Guid id, CancellationToken ct)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == id, ct);
            if (insumo == null) return NotFound();

            insumo.Restaurar();
            await _context.SaveChangesAsync(ct);
            return Ok(new
            {
                insumo.Id,
                insumo.Codigo,
                insumo.Nombre,
                StockActual = insumo.StockActual,
                UnidadMedidaBase = insumo.UnidadMedidaBase.ToString(),
                insumo.CostoUnitarioBaseUSD,
                insumo.PermiteFraccionamiento,
                insumo.Categoria,
                insumo.IsDeleted,
                insumo.OcultoEnTraslados
            });
        }

        [HttpPost("compras")]
        public async Task<IActionResult> RecordPurchase([FromBody] RecordPurchaseDto dto, CancellationToken ct)
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { Message = "No hay ítems en la compra." });
            }

            var username = User.Identity?.Name ?? "System";
            var principalSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;

            decimal totalCompraUSD = 0m;

            foreach (var item in dto.Items)
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == item.InsumoId, ct);
                if (insumo == null)
                {
                    return BadRequest(new { Message = $"No se encontró el insumo con ID {item.InsumoId}." });
                }

                if (item.Cantidad <= 0)
                {
                    return BadRequest(new { Message = $"La cantidad a ingresar para el insumo '{insumo.Nombre}' debe ser mayor a 0." });
                }

                totalCompraUSD += Math.Round(item.Cantidad * item.PrecioCostoUSD, 2);

                insumo.ActualizarDetalles(
                    insumo.Nombre,
                    insumo.UnidadMedidaBase,
                    item.PrecioCostoUSD,
                    insumo.PermiteFraccionamiento,
                    insumo.Categoria
                );

                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == item.InsumoId && s.SedeId == principalSedeId, ct);
                if (stockSede == null)
                {
                    stockSede = new StockSede(item.InsumoId, principalSedeId, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                stockSede.RegistrarMovimientoStock(item.Cantidad, insumo.PermiteFraccionamiento);

                var mov = new MovimientoInsumo(
                    item.InsumoId,
                    principalSedeId,
                    "Ingreso",
                    item.Cantidad,
                    insumo.UnidadMedidaBase,
                    item.Cantidad,
                    username,
                    $"Compra de insumos registrada a costo unitario ${item.PrecioCostoUSD} USD. Prov: {dto.ProveedorNombre ?? "General"}"
                );
                _context.MovimientosInsumo.Add(mov);
            }

            // Registrar automáticamente la Cuenta por Pagar / Orden de Compra
            var provNombre = !string.IsNullOrWhiteSpace(dto.ProveedorNombre) ? dto.ProveedorNombre.Trim() : "Proveedor General";
            var numFact = !string.IsNullOrWhiteSpace(dto.NumeroFactura) ? dto.NumeroFactura.Trim() : $"FAC-{DateTime.Now:yyyyMMddHHmmss}";
            var tasa = dto.TasaCambio.HasValue && dto.TasaCambio > 0 ? dto.TasaCambio.Value : 50.00m;

            var ordenCompra = new SistemaSatHospitalario.Core.Domain.Entities.Admision.OrdenCompraInventario(
                numFact,
                provNombre,
                DateTime.Now,
                totalCompraUSD > 0 ? totalCompraUSD : 1.00m,
                tasa,
                null,
                $"Ingreso atómico de compra de insumos registrado por {username}."
            );
            _context.OrdenesCompraInventario.Add(ordenCompra);

            await _context.SaveChangesAsync(ct);
            return Ok(new { Success = true });
        }
    }

    public class CreateInsumoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal StockInicial { get; set; }
        public UnidadMedida UnidadMedidaBase { get; set; }
        public decimal CostoUnitarioBaseUSD { get; set; }
        public bool PermiteFraccionamiento { get; set; } = true;
        public string Categoria { get; set; } = "Medicamento";
    }

    public class UpdateInsumoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public UnidadMedida UnidadMedidaBase { get; set; }
        public decimal CostoUnitarioBaseUSD { get; set; }
        public bool PermiteFraccionamiento { get; set; } = true;
        public string Categoria { get; set; } = "Medicamento";
    }

    public class CreatePrincipioActivoDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class VincularPrincipioActivoDto
    {
        public Guid PrincipioActivoId { get; set; }
        public string Concentracion { get; set; } = string.Empty;
    }

    public class RegistrarDescarteRequestDto
    {
        public Guid InsumoId { get; set; }
        public Guid? SedeId { get; set; }
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class RecordPurchaseDto
    {
        public Guid? SedeId { get; set; }
        public string? ProveedorNombre { get; set; }
        public string? NumeroFactura { get; set; }
        public decimal? TasaCambio { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioCostoUSD { get; set; }
    }

    public class RecordMovementDto
    {
        public Guid InsumoId { get; set; }
        public Guid SedeId { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal CantidadOriginal { get; set; }
        public UnidadMedida UnidadMedidaOriginal { get; set; }
        public string? Usuario { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class PerformClosingDto
    {
        public Guid SedeId { get; set; }
        public string? Usuario { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public List<CierreDetalleInputDto> Detalles { get; set; } = new();
    }
}
