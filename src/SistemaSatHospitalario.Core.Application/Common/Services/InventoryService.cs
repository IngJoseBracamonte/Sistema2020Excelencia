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
            Guid? sedeId,
            CancellationToken cancellationToken)
        {
            if (cantidadServicio <= 0) return;

            // [PHASE-6] Check if the service requires inventory deduction
            var baseService = await _context.ServiciosClinicos
                .FirstOrDefaultAsync(s => s.Id == serviceId || s.Codigo == serviceCodigo, cancellationToken);
            if (baseService != null && !baseService.RequiereInventario)
            {
                return; // Omit inventory deduction as requested
            }

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
            var targetSedeId = sedeId;
            if (targetSedeId == null || targetSedeId == Guid.Empty)
            {
                var cuenta = await _context.CuentasServicios
                    .Include(c => c.AreaClinica)
                    .FirstOrDefaultAsync(c => c.Id == cuentaId, cancellationToken);
                
                if (cuenta != null)
                {
                    if (cuenta.AreaClinica != null)
                    {
                        targetSedeId = cuenta.AreaClinica.SedeId;
                    }
                    else
                    {
                        targetSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.ResolveSedeInventario(cuenta.TipoIngreso, cuenta.SubAreaClinica);
                    }
                }

                if (targetSedeId == null || targetSedeId == Guid.Empty)
                {
                    targetSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;
                }
            }

            foreach (var recipe in recipes)
            {
                await DeductStockForRecipeAsync(detalleId, cuentaId, targetSedeId.Value, serviceDescripcion, cantidadServicio, usuarioCarga, recipe, cancellationToken);
            }        }

        private async Task DeductStockForRecipeAsync(
            Guid detalleId,
            Guid cuentaId,
            Guid targetSedeId,
            string serviceDescripcion,
            decimal cantidadServicio,
            string usuarioCarga,
            ServicioInsumoReceta recipe,
            CancellationToken cancellationToken)
        {
            if (recipe.Insumo == null) return;

             // 2. Conversion factor from consumption unit to base stock unit
            decimal conversionFactor = GetConversionFactor(recipe.UnidadMedidaConsumo, recipe.Insumo.UnidadMedidaBase);
            decimal qtyBaseNeeded = recipe.Cantidad * conversionFactor * cantidadServicio;

            // Check if target Sede is predetermined/dedicated (with stock physical local tracker)
            var isDedicatedSede = targetSedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal ||
                                  targetSedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Emergencia ||
                                  targetSedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Hospitalizacion ||
                                  targetSedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI;
            
            Guid stockDeductionSedeId = targetSedeId;
            if (!isDedicatedSede)
            {
                stockDeductionSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;
            }

            // 3. Find or create StockSede for target Sede
            var stockSede = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == recipe.InsumoId && s.SedeId == stockDeductionSedeId, cancellationToken);
            if (stockSede == null)
            {
                _logger.LogInformation("Inicializando stock JIT en sede para el insumo {InsumoId} en Sede {SedeId}", recipe.InsumoId, stockDeductionSedeId);
                stockSede = new StockSede(recipe.InsumoId, stockDeductionSedeId, 0);
                _context.StocksSedes.Add(stockSede);
            }

            // Deduct stock (allowing negative values per user requirements)
            stockSede.RegistrarMovimientoStock(-qtyBaseNeeded, recipe.Insumo.PermiteFraccionamiento);
            _logger.LogInformation("Stock deducido para Insumo {InsumoNombre} ({InsumoCodigo}) en Sede {SedeId}. Nuevo stock: {StockActual}",
                recipe.Insumo.Nombre, recipe.Insumo.Codigo, stockDeductionSedeId, stockSede.StockActual);

            // 4. Warning check
            if (stockSede.StockActual < 0)
            {
                _logger.LogWarning("ALERTA DE STOCK INSUFICIENTE EN SEDE: El insumo '{InsumoNombre}' ({InsumoCodigo}) ha quedado en stock negativo ({StockActual} {UnidadMedida}) tras consumir {Consumido} {UnidadMedida} para el servicio '{ServicioDescripcion}' (Detalle Cuenta: {DetalleId}) en Sede {SedeId}.",
                    recipe.Insumo.Nombre, recipe.Insumo.Codigo, stockSede.StockActual, recipe.Insumo.UnidadMedidaBase, qtyBaseNeeded, recipe.Insumo.UnidadMedidaBase, serviceDescripcion, detalleId, targetSedeId);
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
                targetSedeId,
                "Consumo",
                -qtyBaseNeeded,
                recipe.UnidadMedidaConsumo,
                recipe.Cantidad * cantidadServicio,
                usuarioCarga,
                $"Consumo automático por facturación de servicio {serviceDescripcion} (Cuenta ID: {cuentaId})"
            );
            _context.MovimientosInsumo.Add(movimiento);
        }

        public async Task RecordMovementAsync(
            Guid insumoId,
            Guid sedeId,
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

            // Find or create StockSede JIT
            var stockSede = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == insumoId && s.SedeId == sedeId, cancellationToken);
            if (stockSede == null)
            {
                stockSede = new StockSede(insumoId, sedeId, 0);
                _context.StocksSedes.Add(stockSede);
            }

            stockSede.RegistrarMovimientoStock(qtyBase, insumo.PermiteFraccionamiento);

            if (stockSede.StockActual < 0)
            {
                _logger.LogWarning("ALERTA DE STOCK INSUFICIENTE EN SEDE: El insumo '{InsumoNombre}' ({InsumoCodigo}) ha quedado en stock negativo ({StockActual} {UnidadMedida}) tras registrar un movimiento de tipo '{TipoMovimiento}' de {Consumido} {UnidadOriginal} en Sede {SedeId}.",
                    insumo.Nombre, insumo.Codigo, stockSede.StockActual, insumo.UnidadMedidaBase, tipoMovimiento, cantidadOriginal, unidadMedidaOriginal, sedeId);
            }

            var movimiento = new MovimientoInsumo(
                insumoId,
                sedeId,
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
            Guid sedeId,
            string usuario,
            string observaciones,
            List<CierreDetalleInputDto> detalles,
            CancellationToken cancellationToken)
        {
            var closure = new CierreInventario(sedeId, usuario, observaciones);
            _context.CierresInventario.Add(closure);

            foreach (var item in detalles)
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == item.InsumoId, cancellationToken);
                if (insumo == null) continue;

                // Find or create StockSede JIT
                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == item.InsumoId && s.SedeId == sedeId, cancellationToken);
                if (stockSede == null)
                {
                    stockSede = new StockSede(item.InsumoId, sedeId, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                decimal stockTeorico = stockSede.StockActual;
                decimal stockReal = item.StockReal;
                decimal variance = stockReal - stockTeorico;

                stockSede.EstablecerStockCierre(stockReal);

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
                    sedeId,
                    "AjusteCierre",
                    variance,
                    insumo.UnidadMedidaBase,
                    variance,
                    usuario,
                    $"Ajuste automático por cierre de inventario en Sede {sedeId}. Diferencia (Fisico - Teorico) = {variance} {insumo.UnidadMedidaBase}."
                );
                _context.MovimientosInsumo.Add(adjustmentMov);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DispatchPedidoAsync(
            Guid pedidoId,
            string usuario,
            CancellationToken cancellationToken)
        {
            var pedido = await _context.PedidosInterSede
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, cancellationToken);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"No se encontró el pedido inter-sede con ID {pedidoId}");
            }

            if (pedido.Estado != EstadoPedidoInterSede.Solicitado)
            {
                throw new InvalidOperationException("El pedido no está en un estado que permita despacho.");
            }

            // Validar stock disponible en la sede proveedora y descontar stock
            foreach (var detalle in pedido.Detalles)
            {
                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == detalle.InsumoId && s.SedeId == pedido.SedeProveedoraId, cancellationToken);

                if (stockSede == null)
                {
                    _logger.LogInformation("Inicializando stock JIT para despacho en Sede Proveedora {SedeId} para Insumo {InsumoId}", pedido.SedeProveedoraId, detalle.InsumoId);
                    stockSede = new StockSede(detalle.InsumoId, pedido.SedeProveedoraId, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                var stockActual = stockSede.StockActual;
                if (stockActual < detalle.CantidadSolicitada)
                {
                    throw new InvalidOperationException($"Stock insuficiente de '{detalle.Insumo.Nombre}' en la sede proveedora. Solicitado: {detalle.CantidadSolicitada}, Disponible: {stockActual}");
                }

                // Descuenta stock
                stockSede.RegistrarMovimientoStock(-detalle.CantidadSolicitada, detalle.Insumo.PermiteFraccionamiento);
                _logger.LogInformation("Stock transferido desde Sede Proveedora {SedeId}. Insumo: {InsumoId}, Cantidad: {Cantidad}", pedido.SedeProveedoraId, detalle.InsumoId, detalle.CantidadSolicitada);
                detalle.SetDespachado(detalle.CantidadSolicitada);

                // Registrar movimiento de salida
                var movimiento = new MovimientoInsumo(
                    detalle.InsumoId,
                    pedido.SedeProveedoraId,
                    "TransferenciaSalida",
                    -detalle.CantidadSolicitada,
                    detalle.Insumo.UnidadMedidaBase,
                    detalle.CantidadSolicitada,
                    usuario,
                    $"Despacho de pedido inter-sede {pedido.Correlativo} hacia sede solicitante"
                );
                _context.MovimientosInsumo.Add(movimiento);
            }

            pedido.CambiarEstado(EstadoPedidoInterSede.Despachado);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ReceivePedidoAsync(
            Guid pedidoId,
            string usuario,
            Dictionary<Guid, decimal> discrepancias,
            CancellationToken cancellationToken)
        {
            var pedido = await _context.PedidosInterSede
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, cancellationToken);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"No se encontró el pedido inter-sede con ID {pedidoId}");
            }

            if (pedido.Estado != EstadoPedidoInterSede.Despachado)
            {
                throw new InvalidOperationException("El pedido no está en un estado que permita recepción.");
            }

            foreach (var detalle in pedido.Detalles)
            {
                // Determinar la cantidad realmente recibida (se permite discrepancia/recepción parcial)
                decimal cantidadRecibida = detalle.CantidadDespachada;
                if (discrepancias != null && discrepancias.TryGetValue(detalle.InsumoId, out decimal cantDiscrepancia))
                {
                    cantidadRecibida = cantDiscrepancia;
                }

                // Incrementa stock en la sede solicitante (destino)
                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == detalle.InsumoId && s.SedeId == pedido.SedeSolicitanteId, cancellationToken);
                
                if (stockSede == null)
                {
                    stockSede = new StockSede(detalle.InsumoId, pedido.SedeSolicitanteId, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                stockSede.RegistrarMovimientoStock(cantidadRecibida, detalle.Insumo.PermiteFraccionamiento);
                detalle.SetRecibido(cantidadRecibida);

                // Registrar movimiento de entrada
                var movimiento = new MovimientoInsumo(
                    detalle.InsumoId,
                    pedido.SedeSolicitanteId,
                    "TransferenciaEntrada",
                    cantidadRecibida,
                    detalle.Insumo.UnidadMedidaBase,
                    cantidadRecibida,
                    usuario,
                    $"Recepción de pedido inter-sede {pedido.Correlativo}"
                );
                _context.MovimientosInsumo.Add(movimiento);

                // Si hay discrepancia, se podría devolver el stock no entregado/perdido al proveedor o registrar merma/ajuste
                // De acuerdo a las especificaciones, solo se anota la discrepancia en la cantidad recibida
            }

            pedido.CambiarEstado(EstadoPedidoInterSede.Recibido);
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
