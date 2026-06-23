using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Common.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IApplicationDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task DeductInventoryForServiceDetailAsync(
            Guid detalleId,
            Guid serviceId,
            string serviceCodigo,
            string serviceDescripcion,
            decimal cantidadServicio,
            string usuarioCarga,
            Guid cuentaId,
            CancellationToken cancellationToken)
        {
            if (cantidadServicio <= 0) return;

            // 1. Fetch recipes matching this service (by ID or Code)
            var recipes = await _context.ServiciosInsumoRecetas
                .Include(r => r.Insumo)
                .Where(r => (serviceId != Guid.Empty && r.ServicioClinicoId == serviceId) ||
                            (!string.IsNullOrEmpty(serviceCodigo) && r.ServicioCodigo == serviceCodigo))
                .ToListAsync(cancellationToken);

            if (recipes == null || !recipes.Any())
            {
                return; // No recipe mapping for this service, skip inventory deduction
            }

            foreach (var recipe in recipes)
            {
                if (recipe.Insumo == null) continue;

                // 2. Conversion factor from consumption unit to base stock unit
                decimal conversionFactor = GetConversionFactor(recipe.UnidadMedidaConsumo, recipe.Insumo.UnidadMedidaBase);
                decimal qtyBaseNeeded = recipe.Cantidad * conversionFactor * cantidadServicio;

                // 3. Deduct stock (allowing negative values per user requirements)
                recipe.Insumo.RegistrarMovimientoStock(-qtyBaseNeeded);

                // 4. Warning check
                if (recipe.Insumo.StockActual < 0)
                {
                    _logger.LogWarning("ALERTA DE STOCK INSUFICIENTE: El insumo '{InsumoNombre}' ({InsumoCodigo}) ha quedado en stock negativo ({StockActual} {UnidadMedida}) tras consumir {Consumido} {UnidadMedida} para el servicio '{ServicioDescripcion}' (Detalle Cuenta: {DetalleId}).",
                        recipe.Insumo.Nombre, recipe.Insumo.Codigo, recipe.Insumo.StockActual, recipe.Insumo.UnidadMedidaBase, qtyBaseNeeded, recipe.Insumo.UnidadMedidaBase, serviceDescripcion, detalleId);
                }

                // 5. Add ConsumoServicioRealizado
                decimal costTotalUSD = recipe.Insumo.CostoUnitarioBaseUSD * qtyBaseNeeded;
                var consumo = new ConsumoServicioRealizado(
                    detalleId,
                    recipe.InsumoId,
                    qtyBaseNeeded,
                    costTotalUSD
                );
                _context.ConsumosServiciosRealizados.Add(consumo);

                // 6. Add MovimientoInsumo (Immutable Audit Ledger)
                var movimiento = new MovimientoInsumo(
                    recipe.InsumoId,
                    "Consumo",
                    -qtyBaseNeeded,
                    recipe.UnidadMedidaConsumo,
                    recipe.Cantidad * cantidadServicio,
                    usuarioCarga,
                    $"Consumo automático por facturación de servicio {serviceDescripcion} (Cuenta ID: {cuentaId})"
                );
                _context.MovimientosInsumo.Add(movimiento);
            }
        }

        public async Task RecordMovementAsync(
            Guid insumoId,
            string tipoMovimiento,
            decimal cantidadOriginal,
            UnidadMedida unidadMedidaOriginal,
            string usuario,
            string motivo,
            CancellationToken cancellationToken)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == insumoId, cancellationToken);
            if (insumo == null)
            {
                throw new KeyNotFoundException($"No se encontró el insumo con ID {insumoId}");
            }

            decimal conversionFactor = GetConversionFactor(unidadMedidaOriginal, insumo.UnidadMedidaBase);
            decimal qtyBase = cantidadOriginal * conversionFactor;

            if (tipoMovimiento.Equals("Descarte", StringComparison.OrdinalIgnoreCase))
            {
                qtyBase = -qtyBase;
            }

            insumo.RegistrarMovimientoStock(qtyBase);

            if (insumo.StockActual < 0)
            {
                _logger.LogWarning("ALERTA DE STOCK INSUFICIENTE: El insumo '{InsumoNombre}' ({InsumoCodigo}) ha quedado en stock negativo ({StockActual} {UnidadMedida}) tras registrar un movimiento de tipo '{TipoMovimiento}' de {Consumido} {UnidadOriginal}.",
                    insumo.Nombre, insumo.Codigo, insumo.StockActual, insumo.UnidadMedidaBase, tipoMovimiento, cantidadOriginal, unidadMedidaOriginal);
            }

            var movimiento = new MovimientoInsumo(
                insumoId,
                tipoMovimiento,
                qtyBase,
                unidadMedidaOriginal,
                cantidadOriginal,
                usuario,
                motivo
            );

            _context.MovimientosInsumo.Add(movimiento);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task PerformClosingAsync(
            string usuario,
            string observaciones,
            List<CierreDetalleInputDto> detalles,
            CancellationToken cancellationToken)
        {
            var closure = new CierreInventario(usuario, observaciones);
            _context.CierresInventario.Add(closure);

            foreach (var item in detalles)
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == item.InsumoId, cancellationToken);
                if (insumo == null) continue;

                decimal stockTeorico = insumo.StockActual;
                decimal stockReal = item.StockReal;
                decimal variance = stockReal - stockTeorico;

                insumo.EstablecerStockCierre(stockReal);

                var detail = new CierreInventarioDetalle(
                    closure.Id,
                    insumo.Id,
                    stockTeorico,
                    stockReal,
                    insumo.CostoUnitarioBaseUSD
                );
                _context.CierresInventarioDetalles.Add(detail);

                var adjustmentMov = new MovimientoInsumo(
                    insumo.Id,
                    "AjusteCierre",
                    variance,
                    insumo.UnidadMedidaBase,
                    variance,
                    usuario,
                    $"Ajuste automático por cierre de inventario. Diferencia (Fisico - Teorico) = {variance} {insumo.UnidadMedidaBase}."
                );
                _context.MovimientosInsumo.Add(adjustmentMov);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public static decimal GetConversionFactor(UnidadMedida origen, UnidadMedida destino)
        {
            if (origen == destino) return 1.0m;

            // Volume conversions: L to ML and vice-versa
            if (origen == UnidadMedida.L && destino == UnidadMedida.ML) return 1000m;
            if (origen == UnidadMedida.ML && destino == UnidadMedida.L) return 0.001m;

            // Mass conversions: KG, G, DG, MG to Grams (G)
            decimal origenEnGramos = origen switch
            {
                UnidadMedida.KG => 1000m,
                UnidadMedida.G => 1m,
                UnidadMedida.DG => 0.1m,
                UnidadMedida.MG => 0.001m,
                _ => 1m
            };

            decimal gramosADestino = destino switch
            {
                UnidadMedida.KG => 0.001m,
                UnidadMedida.G => 1m,
                UnidadMedida.DG => 10m,
                UnidadMedida.MG => 1000m,
                _ => 1m
            };

            bool esMasaOrigen = origen == UnidadMedida.KG || origen == UnidadMedida.G || origen == UnidadMedida.DG || origen == UnidadMedida.MG;
            bool esMasaDestino = destino == UnidadMedida.KG || destino == UnidadMedida.G || destino == UnidadMedida.DG || destino == UnidadMedida.MG;

            if (esMasaOrigen && esMasaDestino)
            {
                return origenEnGramos * gramosADestino;
            }

            // Default fallback
            return 1.0m;
        }
    }
}
