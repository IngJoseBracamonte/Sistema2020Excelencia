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
using SistemaSatHospitalario.Core.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IApplicationDbContext context, 
            IInventoryService inventoryService, 
            IMediator mediator,
            ILogger<InventoryController> logger)
        {
            _context = context;
            _inventoryService = inventoryService;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 3FN: resuelve la categoría normalizada. Prioridad: 1) CategoriaInsumoId (FK),
        /// 2) coincidencia única por nombre (case/space-insensitive) sobre el texto legacy.
        /// Devuelve null si no hay match único (el insumo conserva solo el alias de texto).
        /// </summary>
        private async Task<CategoriaInsumo?> ResolverCategoriaInsumoAsync(Guid? categoriaInsumoId, string? categoriaTexto, CancellationToken ct)
        {
            if (categoriaInsumoId.HasValue)
            {
                return await _context.CategoriasInsumo
                    .FirstOrDefaultAsync(c => c.Id == categoriaInsumoId.Value && c.Activo, ct);
            }

            if (string.IsNullOrWhiteSpace(categoriaTexto))
            {
                return null;
            }

            var normalizado = categoriaTexto.Trim().ToUpper();
            var coincidencias = await _context.CategoriasInsumo
                .Where(c => c.Activo && c.Nombre.ToUpper() == normalizado)
                .Take(2)
                .ToListAsync(ct);

            return coincidencias.Count == 1 ? coincidencias[0] : null;
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
                .Include(i => i.CategoriaInsumo)
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
                    Categoria = i.CategoriaInsumo != null ? i.CategoriaInsumo.Nombre : i.Categoria,
                    i.CategoriaInsumoId,
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
                .Include(i => i.CategoriaInsumo)
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
                Categoria = insumo.CategoriaInsumo?.Nombre ?? insumo.Categoria,
                insumo.CategoriaInsumoId,
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
            await ReconciliarDespachosPendientesDeEntradaAsync(sedeId, ct);

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
            await ReconciliarDespachosPendientesDeEntradaAsync(sedeId, ct);

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

        private async Task ReconciliarDespachosPendientesDeEntradaAsync(Guid sedeId, CancellationToken ct)
        {
            var pedidosDespachados = await _context.PedidosInterSede
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Insumo)
                .Where(p => p.SedeSolicitanteId == sedeId && p.Estado == EstadoPedidoInterSede.Recibido)
                .ToListAsync(ct);

            bool hayCambios = false;
            foreach (var ped in pedidosDespachados)
            {
                if (!string.IsNullOrEmpty(ped.Observaciones) && ped.Observaciones.Contains("[GASTO_INTERNO_LABORATORIO]", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool tieneEntrada = await _context.MovimientosInsumo
                    .AnyAsync(m => m.SedeId == sedeId && m.TipoMovimiento == TipoMovimientoInsumo.TransferenciaEntrada && m.Motivo != null && m.Motivo.Contains(ped.Correlativo), ct);

                if (!tieneEntrada)
                {
                    foreach (var det in ped.Detalles)
                    {
                        var cant = det.CantidadDespachada > 0 ? det.CantidadDespachada : det.CantidadSolicitada;
                        if (cant > 0)
                        {
                            var stockSede = await _context.StocksSedes
                                .FirstOrDefaultAsync(s => s.InsumoId == det.InsumoId && s.SedeId == sedeId, ct);

                            if (stockSede == null)
                            {
                                stockSede = new StockSede(det.InsumoId, sedeId, 0);
                                _context.StocksSedes.Add(stockSede);
                            }

                            stockSede.RegistrarMovimientoStock(cant, det.Insumo.PermiteFraccionamiento);

                            var movEntrada = new MovimientoInsumo(
                                det.InsumoId,
                                sedeId,
                                "TransferenciaEntrada",
                                cant,
                                det.Insumo.UnidadMedidaBase,
                                cant,
                                ped.UsuarioCreador ?? "admin",
                                $"Recepción por despacho de pedido inter-sede {ped.Correlativo}"
                            );
                            _context.MovimientosInsumo.Add(movEntrada);
                            hayCambios = true;
                        }
                    }
                }
            }

            // Reconciliar devoluciones de cirugía pendientes hacia Sede Principal
            if (sedeId == SeedConstants.SedeId_Principal)
            {
                var devolucionesCirugia = await _context.InsumosCirugiasPacientes
                    .Include(i => i.Insumo)
                    .Where(i => i.CantidadDevuelta > 0)
                    .ToListAsync(ct);

                foreach (var dev in devolucionesCirugia)
                {
                    bool tieneMov = await _context.MovimientosInsumo
                        .AnyAsync(m => m.InsumoId == dev.InsumoId && m.SedeId == SeedConstants.SedeId_Principal && m.TipoMovimiento == TipoMovimientoInsumo.Ingreso && m.Motivo != null && m.Motivo.Contains(dev.CuentaServicioId.ToString()), ct);

                    if (!tieneMov)
                    {
                        var stockPrincipal = await _context.StocksSedes
                            .FirstOrDefaultAsync(s => s.InsumoId == dev.InsumoId && s.SedeId == SeedConstants.SedeId_Principal, ct);

                        if (stockPrincipal == null)
                        {
                            stockPrincipal = new StockSede(dev.InsumoId, SeedConstants.SedeId_Principal, 0);
                            _context.StocksSedes.Add(stockPrincipal);
                        }

                        stockPrincipal.RegistrarMovimientoStock(dev.CantidadDevuelta, dev.Insumo?.PermiteFraccionamiento ?? true);

                        var movDev = new MovimientoInsumo(
                            dev.InsumoId,
                            SeedConstants.SedeId_Principal,
                            TipoMovimientoInsumo.Ingreso,
                            dev.CantidadDevuelta,
                            dev.Insumo?.UnidadMedidaBase ?? UnidadMedida.UNIDAD,
                            dev.CantidadDevuelta,
                            "admin",
                            $"Devolución de sobrante de cirugía reconciliada (Cuenta: {dev.CuentaServicioId})"
                        );
                        _context.MovimientosInsumo.Add(movDev);
                        hayCambios = true;
                    }
                }
            }

            if (hayCambios)
            {
                await _context.SaveChangesAsync(ct);
            }
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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de Kárdex. SedeId: {SedeId}, InsumoId: {InsumoId}, FechaDesde: {FechaDesde}, FechaHasta: {FechaHasta}", sedeId, insumoId, fechaDesde, fechaHasta);
                return StatusCode(500, new { message = "Error al procesar la consulta de Kárdex.", error = ex.Message });
            }
        }

        [HttpPost("insumos")]
        public async Task<IActionResult> CreateInsumo([FromBody] CreateInsumoDto dto, CancellationToken ct)
        {
            var existingInsumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Codigo.ToLower() == dto.Codigo.Trim().ToLower(), ct);
            if (existingInsumo != null)
            {
                if (existingInsumo.IsDeleted)
                {
                    return Conflict(new
                    {
                        Message = $"El código '{dto.Codigo}' pertenece al insumo desactivado '{existingInsumo.Nombre}'.",
                        InsumoId = existingInsumo.Id,
                        Codigo = existingInsumo.Codigo,
                        Nombre = existingInsumo.Nombre,
                        EstaDesactivado = true
                    });
                }
                else
                {
                    return BadRequest(new 
                    { 
                        Message = $"Ya existe un insumo activo con el código '{dto.Codigo}' ({existingInsumo.Nombre}).",
                        InsumoId = existingInsumo.Id,
                        Codigo = existingInsumo.Codigo,
                        Nombre = existingInsumo.Nombre,
                        EstaDesactivado = false
                    });
                }
            }

            var insumo = new Insumo(dto.Codigo, dto.Nombre, dto.StockInicial, dto.UnidadMedidaBase, dto.CostoUnitarioBaseUSD, dto.PermiteFraccionamiento, dto.Categoria);

            // 3FN: asignar categoría normalizada por FK (tiene prioridad sobre el texto legacy)
            var categoriaInsumo = await ResolverCategoriaInsumoAsync(dto.CategoriaInsumoId, dto.Categoria, ct);
            if (categoriaInsumo != null)
            {
                insumo.AsignarCategoria(categoriaInsumo);
            }

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

            // --- AUTO-CREACIÓN EN MAESTRO DE SERVICIOS (DESACTIVADO, PRECIO 0, CÓDIGO ALEATORIO) ---
            string prefix = (dto.Categoria ?? "").ToUpperInvariant().Contains(TipoServicioConstants.CategoriaMedicamento.ToUpperInvariant()) ? "MED-AUT-" : "INS-AUT-";
            string randomCode;
            do
            {
                 randomCode = $"{prefix}{RandomNumberGenerator.GetInt32(1000, 10000)}";
            } while (await _context.ServiciosClinicos.AnyAsync(s => s.Codigo == randomCode, ct));

            var servicioClinico = new ServicioClinico(
                randomCode,
                dto.Nombre,
                0m, // Sin precio base USD (0), requiere asignación de Admin
                TipoServicioConstants.TipoInsumoMedicamento
            );
            servicioClinico.Activo = false; // Desactivado por defecto
            servicioClinico.UnidadMedida = dto.UnidadMedidaBase.ToString();
            servicioClinico.PermiteFraccionamiento = dto.PermiteFraccionamiento;
            servicioClinico.RequiereInventario = true;

            _context.ServiciosClinicos.Add(servicioClinico);

            // Vinculación de Receta (BOM) con la cantidad de 1 unidad del insumo
            var receta = new ServicioInsumoReceta(
                servicioClinico.Id,
                insumo.Id,
                1m,
                dto.UnidadMedidaBase
            );
            _context.ServiciosInsumoRecetas.Add(receta);

            // Crear Notificación dirigida a Administradores
            var notification = new SistemaSatHospitalario.Core.Domain.Entities.Common.Notification(
                "Nuevo Insumo por Configurar",
                $"Se ha registrado el insumo '{dto.Nombre}'. Se requiere asignar precio de venta y código final en Maestro de Servicios.",
                "Warning",
                null,
                "Administrador",
                $"/catalog?edit={servicioClinico.Id}"
            );
            _context.Notifications.Add(notification);

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
                Categoria = insumo.CategoriaInsumo?.Nombre ?? insumo.Categoria,
                insumo.CategoriaInsumoId,
                insumo.IsDeleted,
                insumo.OcultoEnTraslados,
                ServicioClinicoId = servicioClinico.Id,
                ServicioClinicoCodigo = servicioClinico.Codigo
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

            // 3FN: asignar categoría normalizada por FK (tiene prioridad sobre el texto legacy)
            var categoriaInsumo = await ResolverCategoriaInsumoAsync(dto.CategoriaInsumoId, dto.Categoria, ct);
            if (categoriaInsumo != null)
            {
                insumo.AsignarCategoria(categoriaInsumo);
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
                Categoria = insumo.CategoriaInsumo?.Nombre ?? insumo.Categoria,
                insumo.CategoriaInsumoId,
                insumo.IsDeleted,
                insumo.OcultoEnTraslados
            });
        }

        // --- Categorías de Insumos CRUD ---

        [HttpGet("categorias")]
        public async Task<IActionResult> GetCategorias(CancellationToken ct)
        {
            var categorias = await _context.CategoriasInsumo
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync(ct);
            return Ok(categorias);
        }

        [HttpPost("categorias")]
        public async Task<IActionResult> CreateCategoria([FromBody] CreateCategoriaInsumoDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest(new { Message = "El nombre de la categoría es obligatorio." });
            }

            var nombreNormalizado = dto.Nombre.Trim();
            var existing = await _context.CategoriasInsumo
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombreNormalizado.ToLower(), ct);

            if (existing != null)
            {
                if (!existing.Activo)
                {
                    existing.SetEstado(true);
                    await _context.SaveChangesAsync(ct);
                }
                return Ok(existing);
            }

            var categoria = new CategoriaInsumo(nombreNormalizado, dto.Codigo);
            _context.CategoriasInsumo.Add(categoria);
            await _context.SaveChangesAsync(ct);
            return Ok(categoria);
        }

        [HttpPut("categorias/{id}")]
        public async Task<IActionResult> UpdateCategoria(Guid id, [FromBody] UpdateCategoriaInsumoDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest(new { Message = "El nombre de la categoría es obligatorio." });
            }

            var categoria = await _context.CategoriasInsumo.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (categoria == null)
            {
                return NotFound(new { Message = "Categoría no encontrada." });
            }

            var nombreNormalizado = dto.Nombre.Trim();
            var duplicate = await _context.CategoriasInsumo
                .FirstOrDefaultAsync(c => c.Id != id && c.Nombre.ToLower() == nombreNormalizado.ToLower(), ct);

            if (duplicate != null)
            {
                return BadRequest(new { Message = $"Ya existe otra categoría con el nombre '{nombreNormalizado}'." });
            }

            var nombreAnterior = categoria.Nombre;
            categoria.ActualizarNombre(nombreNormalizado, dto.Codigo);

            if (dto.Activo.HasValue)
            {
                categoria.SetEstado(dto.Activo.Value);
            }

            // 3FN: propagar el nuevo nombre canónico a los insumos vinculados por FK
            // (el alias de texto Categoria se sincroniza dentro de AsignarCategoria)
            var insumosConCategoria = await _context.Insumos
                .Where(i => i.CategoriaInsumoId == categoria.Id || i.Categoria == nombreAnterior)
                .ToListAsync(ct);

            foreach (var insumo in insumosConCategoria)
            {
                insumo.AsignarCategoria(categoria);
            }

            await _context.SaveChangesAsync(ct);
            return Ok(categoria);
        }

        [HttpDelete("categorias/{id}")]
        public async Task<IActionResult> DeleteCategoria(Guid id, CancellationToken ct)
        {
            var categoria = await _context.CategoriasInsumo.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (categoria == null)
            {
                return NotFound(new { Message = "Categoría no encontrada." });
            }

            categoria.SetEstado(false);
            await _context.SaveChangesAsync(ct);
            return NoContent();
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
                new { Id = SistemaSatHospitalario.Core.Domain.Enums.TipoMovimientoInsumo.Ingreso.ToString(), Codigo = "ING", Nombre = "Ingreso / Compra Insumo" },
                new { Id = SistemaSatHospitalario.Core.Domain.Enums.TipoMovimientoInsumo.Consumo.ToString(), Codigo = "ENV", Nombre = "Envío / Consumo Directo Sub-Área" },
                new { Id = SistemaSatHospitalario.Core.Domain.Enums.TipoMovimientoInsumo.Descarte.ToString(), Codigo = "DES", Nombre = "Descarte / Baja de Inventario" },
                new { Id = SistemaSatHospitalario.Core.Domain.Enums.TipoMovimientoInsumo.AjusteCierre.ToString(), Codigo = "AJU", Nombre = "Ajuste Kárdex Cierre / Auditoría" },
                new { Id = SistemaSatHospitalario.Core.Domain.Enums.TipoMovimientoInsumo.TransferenciaSalida.ToString(), Codigo = "TRA", Nombre = "Transferencia entre Sedes" }
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
                Categoria = insumo.CategoriaInsumo?.Nombre ?? insumo.Categoria,
                insumo.CategoriaInsumoId,
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

                // 3FN: solo actualizar datos de precio; la categoría se gestiona vía CategoriaInsumoId
                insumo.ActualizarDetalles(insumo.Nombre, item.PrecioCostoUSD);

                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == item.InsumoId && s.SedeId == principalSedeId, ct);
                if (stockSede == null)
                {
                    stockSede = new StockSede(item.InsumoId, principalSedeId, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                stockSede.RegistrarMovimientoStock(item.Cantidad, insumo.PermiteFraccionamiento);

                var descPres = !string.IsNullOrWhiteSpace(item.PresentacionCompra) ? $" [Pres: {item.PresentacionCompra.Trim()}]" : "";
                var mov = new MovimientoInsumo(
                    item.InsumoId,
                    principalSedeId,
                    "Ingreso",
                    item.Cantidad,
                    insumo.UnidadMedidaBase,
                    item.Cantidad,
                    username,
                    $"Compra de insumos registrada a costo unitario ${item.PrecioCostoUSD} USD.{descPres} Prov: {dto.ProveedorNombre ?? "General"}"
                );
                _context.MovimientosInsumo.Add(mov);
            }

            // 1. Guardar cambios del ingreso de inventario (Stock, Movimientos, Insumos)
            await _context.SaveChangesAsync(ct);

            // 2. Registrar automáticamente la Cuenta por Pagar / Orden de Compra de manera defensiva
            try
            {
                var provNombre = !string.IsNullOrWhiteSpace(dto.ProveedorNombre) ? dto.ProveedorNombre.Trim() : "Proveedor General";
                var numFact = !string.IsNullOrWhiteSpace(dto.NumeroFactura) ? dto.NumeroFactura.Trim() : $"FAC-{DateTime.Now:yyyyMMddHHmmss}";
                var tasa = dto.TasaCambio.HasValue && dto.TasaCambio > 0 ? dto.TasaCambio.Value : 50.00m;

                var ordenCompra = new SistemaSatHospitalario.Core.Domain.Entities.Admision.OrdenCompraInventario(
                    numFact,
                    provNombre,
                    DateTime.Now,
                    totalCompraUSD > 0 ? totalCompraUSD : 1.00m,
                    tasa,
                    dto.ProveedorId,
                    $"Ingreso atómico de compra de insumos registrado por {username}."
                );
                _context.OrdenesCompraInventario.Add(ordenCompra);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo registrar la Orden de Compra en Cuentas por Pagar. Ejecute el script SQL manual si no ha creado las tablas.");
            }

            return Ok(new { Success = true });
        }

        [HttpGet("proveedores")]
        public async Task<IActionResult> GetProveedores([FromQuery] string? q, CancellationToken ct)
        {
            try
            {
                var queryable = _context.Proveedores.AsNoTracking().Where(p => p.Activo);
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var term = q.Trim().ToLower();
                    queryable = queryable.Where(p => p.RazonSocial.ToLower().Contains(term) || p.RIF.ToLower().Contains(term));
                }

                var proveedores = await queryable.OrderBy(p => p.RazonSocial).Take(50).ToListAsync(ct);
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo consultar proveedores de la base de datos MySQL.");
                return Ok(new List<SistemaSatHospitalario.Core.Domain.Entities.Admision.Proveedor>());
            }
        }

        [HttpPost("proveedores")]
        public async Task<IActionResult> CreateProveedor([FromBody] CreateProveedorDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.RIF))
                return BadRequest(new { Message = "El RIF es requerido." });
            if (string.IsNullOrWhiteSpace(dto.RazonSocial))
                return BadRequest(new { Message = "La razón social es requerida." });

            var rifClean = dto.RIF.Trim().ToUpperInvariant();
            try
            {
                if (await _context.Proveedores.AnyAsync(p => p.RIF == rifClean, ct))
                {
                    return BadRequest(new { Message = $"Ya existe un proveedor registrado con el RIF {rifClean}." });
                }

                var proveedor = new SistemaSatHospitalario.Core.Domain.Entities.Admision.Proveedor(dto.RIF, dto.RazonSocial, dto.Direccion, dto.Telefono);
                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync(ct);

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor en la base de datos.");
                return BadRequest(new { Message = "Error al guardar el proveedor. Asegúrese de ejecutar el script SQL manual para crear la tabla 'Proveedores' en MySQL." });
            }
        }

        // --- RECETAS / BOM ENDPOINTS ---

        [HttpGet("recetas")]
        public async Task<IActionResult> GetRecetas(CancellationToken ct)
        {
            try
            {
                var recetas = await (from r in _context.ServiciosInsumoRecetas.AsNoTracking()
                                     join i in _context.Insumos.AsNoTracking() on r.InsumoId equals i.Id into ri
                                     from i in ri.DefaultIfEmpty()
                                     select new
                                     {
                                         Id = r.Id,
                                         ServicioClinicoId = r.ServicioClinicoId,
                                         InsumoId = r.InsumoId,
                                         InsumoNombre = i != null ? i.Nombre : "Insumo Desconocido",
                                         Cantidad = r.Cantidad,
                                         UnidadMedidaConsumo = r.UnidadMedidaConsumo.ToString()
                                     }).ToListAsync(ct);

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener recetas de insumos.");
                return Ok(new List<object>());
            }
        }

        [HttpPost("recetas")]
        public async Task<IActionResult> CreateReceta([FromBody] CreateRecetaDto dto, CancellationToken ct)
        {
            if (dto == null || dto.ServicioClinicoId == Guid.Empty || dto.InsumoId == Guid.Empty || dto.Cantidad <= 0)
            {
                return BadRequest(new { Message = "Parámetros inválidos para registrar la receta." });
            }

            var servicio = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == dto.ServicioClinicoId, ct);
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == dto.InsumoId, ct);
            if (servicio == null || insumo == null)
            {
                return NotFound(new { Message = "El servicio clínico o el insumo no existen." });
            }

            Enum.TryParse<UnidadMedida>(dto.UnidadMedidaConsumo ?? insumo.UnidadMedidaBase.ToString(), true, out var unidad);

            var receta = new ServicioInsumoReceta(
                servicio.Id,
                insumo.Id,
                dto.Cantidad,
                unidad
            );

            _context.ServiciosInsumoRecetas.Add(receta);
            await _context.SaveChangesAsync(ct);

            return Ok(new
            {
                Id = receta.Id,
                ServicioClinicoId = receta.ServicioClinicoId,
                InsumoId = receta.InsumoId,
                InsumoNombre = insumo.Nombre,
                Cantidad = receta.Cantidad,
                UnidadMedidaConsumo = receta.UnidadMedidaConsumo.ToString()
            });
        }

        [HttpDelete("recetas/{id}")]
        public async Task<IActionResult> DeleteReceta(Guid id, CancellationToken ct)
        {
            var receta = await _context.ServiciosInsumoRecetas.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (receta == null) return NotFound(new { Message = "Receta no encontrada." });

            _context.ServiciosInsumoRecetas.Remove(receta);
            await _context.SaveChangesAsync(ct);
            return Ok(new { Message = "Receta eliminada con éxito." });
        }
    }

    public class CreateRecetaDto
    {
        public Guid ServicioClinicoId { get; set; }
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public string? UnidadMedidaConsumo { get; set; }
    }

    public class CreateProveedorDto
    {
        public string RIF { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    public class CreateInsumoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal StockInicial { get; set; }
        public UnidadMedida UnidadMedidaBase { get; set; }
        public decimal CostoUnitarioBaseUSD { get; set; }
        public bool PermiteFraccionamiento { get; set; } = true;
        public string Categoria { get; set; } = TipoServicioConstants.CategoriaMedicamento;
        /// <summary>3FN: FK al catálogo CategoriasInsumo. Si se envía, tiene prioridad sobre el texto Categoria.</summary>
        public Guid? CategoriaInsumoId { get; set; }
    }

    public class UpdateInsumoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public UnidadMedida UnidadMedidaBase { get; set; }
        public decimal CostoUnitarioBaseUSD { get; set; }
        public bool PermiteFraccionamiento { get; set; } = true;
        public string Categoria { get; set; } = TipoServicioConstants.CategoriaMedicamento;
        /// <summary>3FN: FK al catálogo CategoriasInsumo. Si se envía, tiene prioridad sobre el texto Categoria.</summary>
        public Guid? CategoriaInsumoId { get; set; }
    }

    public class CreateCategoriaInsumoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Codigo { get; set; }
    }

    public class UpdateCategoriaInsumoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Codigo { get; set; }
        public bool? Activo { get; set; }
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
        public Guid? ProveedorId { get; set; }
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
        public string? PresentacionCompra { get; set; }
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
