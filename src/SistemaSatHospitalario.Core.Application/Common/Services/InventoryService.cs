using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
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
            Guid? usuarioCargaId,   // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Guid cuentaId,
            Guid? sedeId,
            CancellationToken cancellationToken)
        {
            if (cantidadServicio <= 0) return;

            // Check if the service requires inventory deduction
            var baseService = await _context.ServiciosClinicos
                .FirstOrDefaultAsync(s => s.Id == serviceId || s.Codigo == serviceCodigo, cancellationToken);
            if (baseService != null && !baseService.RequiereInventario)
            {
                return; // Omit inventory deduction as requested
            }

            // Fetch recipes through the normalized service relationship.
            var recipes = await _context.ServiciosInsumoRecetas
                .Include(r => r.Insumo)
                .Where(r => serviceId != Guid.Empty && r.ServicioClinicoId == serviceId)
                .ToListAsync(cancellationToken);

            if (recipes == null || !recipes.Any())
            {
                Insumo? directInsumo = null;
                if (_context.Insumos != null)
                {
                    directInsumo = await _context.Insumos
                        .FirstOrDefaultAsync(i => (serviceId != Guid.Empty && i.Id == serviceId) ||
                                                  (!string.IsNullOrEmpty(serviceCodigo) && i.Codigo == serviceCodigo), cancellationToken);
                }

                if (directInsumo != null)
                {
                    var selfRecipe = new ServicioInsumoReceta(
                        directInsumo.Id,
                        directInsumo.Id,
                        1m,
                        (UnidadMedidaEnum)directInsumo.UnidadMedidaId
                    )
                    {
                        Insumo = directInsumo
                    };
                    recipes = new List<ServicioInsumoReceta> { selfRecipe };
                }
                else
                {
                    return;
                }
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
                        targetSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.ResolveSedeInventario(cuenta.TipoIngresoNav.Nombre, cuenta.SubAreaClinica);
                    }
                }

                if (targetSedeId == null || targetSedeId == Guid.Empty)
                {
                    targetSedeId = SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;
                }
            }

            int maxRetries = 3;
            int delayMs = 100;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var hasExistingTransaction = _context.Database?.CurrentTransaction != null;
                var isInMemory = _context.Database?.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
                var transaction = (hasExistingTransaction || isInMemory)
                    ? null
                    : await _context.BeginTransactionAsync(cancellationToken);
                try
                {
                    foreach (var recipe in recipes)
                    {
                        if (recipe.Insumo == null) continue;

                        decimal conversionFactor = GetConversionFactor((UnidadMedidaEnum)recipe.UnidadMedidaConsumoId, (UnidadMedidaEnum)recipe.Insumo.UnidadMedidaId);
                        decimal qtyBaseNeeded = recipe.Cantidad * conversionFactor * cantidadServicio;

                        Guid stockDeductionSedeId = targetSedeId ?? SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;

                        var stockSede = await _context.StocksSedes
                            .FirstOrDefaultAsync(s => s.InsumoId == recipe.InsumoId && s.SedeId == stockDeductionSedeId, cancellationToken);
                        if (stockSede == null)
                        {
                            _logger.LogInformation("Inicializando stock JIT en sede para el insumo {InsumoId} en Sede {SedeId}", recipe.InsumoId, stockDeductionSedeId);
                            stockSede = new StockSede(recipe.InsumoId, stockDeductionSedeId, 0);
                            _context.StocksSedes.Add(stockSede);
                        }

                        stockSede.RegistrarMovimientoStock(-qtyBaseNeeded, recipe.Insumo.PermiteFraccionamiento);

                        if (stockSede.StockActual < 0)
                        {
                            _logger.LogWarning("ALERTA DE STOCK INSUFICIENTE EN SEDE: El insumo '{InsumoNombre}' ({InsumoCodigo}) ha quedado en stock negativo ({StockActual} {UnidadMedida}) tras consumir {Consumido} {UnidadMedida} para el servicio '{ServicioDescripcion}' (Detalle Cuenta: {DetalleId}) en Sede {SedeId}.",
                                recipe.Insumo.Nombre, recipe.Insumo.Codigo, stockSede.StockActual, recipe.Insumo.UnidadMedidaNav.Nombre, qtyBaseNeeded, recipe.Insumo.UnidadMedidaNav.Nombre, serviceDescripcion, detalleId, targetSedeId);
                        }

                        decimal costTotalUSD = recipe.Insumo.CostoUnitarioBaseUSD * qtyBaseNeeded;
                        var consumo = new ConsumoServicioRealizado(
                            detalleId,
                            recipe.InsumoId,
                            qtyBaseNeeded,
                            costTotalUSD
                        );
                        _context.ConsumosServiciosRealizados.Add(consumo);

                        var movimiento = new MovimientoInsumo(
                            recipe.InsumoId,
                            targetSedeId.Value,
                            TipoMovimientoInsumo.Consumo,
                            -qtyBaseNeeded,
                            (UnidadMedidaEnum)recipe.UnidadMedidaConsumoId,
                            recipe.Cantidad * cantidadServicio,
                            $"Consumo automático por facturación de servicio {serviceDescripcion} (Cuenta ID: {cuentaId})"
                        );
                        _context.MovimientosInsumo.Add(movimiento);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                    if (transaction != null)
                    {
                        await transaction.CommitAsync(cancellationToken);
                    }
                    break;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }
                    if (attempt == maxRetries)
                    {
                        _logger.LogError("Se agotaron los {MaxRetries} reintentos de concurrencia al deducir inventario para la cuenta {CuentaId}", maxRetries, cuentaId);
                        throw;
                    }
                    _logger.LogWarning("Conflicto de concurrencia detectado (intento {Attempt}/{MaxRetries}). Reintentando...", attempt, maxRetries);
                    await Task.Delay(delayMs * (int)Math.Pow(2, attempt - 1), cancellationToken);
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }
                    _logger.LogError(ex, "Fallo no recuperable al deducir inventario para la cuenta {CuentaId}", cuentaId);
                    throw;
                }
            }
        }

        public async Task RecordMovementAsync(
            Guid insumoId,
            Guid sedeId,
            string tipoMovimiento,
            decimal cantidadOriginal,
            UnidadMedidaEnum unidadMedidaOriginal,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            string motivo,
            CancellationToken cancellationToken)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == insumoId, cancellationToken);
            if (insumo == null)
            {
                throw new KeyNotFoundException($"No se encontró el insumo con ID {insumoId}");
            }

            decimal conversionFactor = GetConversionFactor(unidadMedidaOriginal, Domain.Constants.UnidadMedidaConstants.ToEnum(insumo.UnidadMedidaId));
            decimal qtyBase = cantidadOriginal * conversionFactor;

            if (tipoMovimiento.Equals("Descarte", StringComparison.OrdinalIgnoreCase))
            {
                qtyBase = -qtyBase;
            }

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
                    insumo.Nombre, insumo.Codigo, stockSede.StockActual, insumo.UnidadMedidaNav.Nombre, tipoMovimiento, cantidadOriginal, unidadMedidaOriginal, sedeId);
            }

            var movimiento = new MovimientoInsumo(
                insumoId,
                sedeId,
                tipoMovimiento,
                qtyBase,
                unidadMedidaOriginal,
                cantidadOriginal,
                motivo,
                usuarioId,
                null
            );

            _context.MovimientosInsumo.Add(movimiento);

            await _context.SaveChangesAsync(cancellationToken);
        }

       
        public async Task PerformClosingAsync(
            Guid sedeId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            string observaciones,
            List<CierreDetalleInputDto> detalles,
            CancellationToken cancellationToken)
        {
            var closure = new CierreInventario(sedeId, usuarioId, observaciones);
            _context.CierresInventario.Add(closure);

            foreach (var item in detalles)
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == item.InsumoId, cancellationToken);
                if (insumo == null) continue;

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
                   TipoMovimientoInsumo.AjusteCierre,
                    variance,
                    (UnidadMedidaEnum)insumo.UnidadMedidaId,
                    variance,
                    observaciones,
                    usuarioId,
                    null
                );
                _context.MovimientosInsumo.Add(adjustmentMov);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DispatchPedidoAsync(
            Guid pedidoId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Dictionary<Guid, decimal>? cantidadesAprobadas = null,
            Dictionary<Guid, string>? observacionesPorDetalle = null,
            CancellationToken cancellationToken = default)
        {
            int retries = 0;
            const int maxRetries = 3;

            while (true)
            {
                try
                {
                    var pedido = await _context.PedidosInterSede
                        .Include(p => p.Detalles)
                            .ThenInclude(d => d.Insumo)
                        .FirstOrDefaultAsync(p => p.Id == pedidoId, cancellationToken);

                    if (pedido == null)
                    {
                        throw new KeyNotFoundException($"No se encontró el pedido inter-sede con ID {pedidoId}");
                    }

                    if (pedido.Estado != EstadoPedidoInterSedeConstants.Solicitado)
                    {
                        throw new InvalidOperationException("El pedido no está en un estado que permita despacho.");
                    }

                    bool esGastoInterno = !string.IsNullOrEmpty(pedido.Observaciones) && 
                        pedido.Observaciones.Contains("[GASTO_INTERNO_LABORATORIO]", StringComparison.OrdinalIgnoreCase);

                    bool esCirugiaAdHoc = (pedido.SedeSolicitanteId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Cirugia) ||
                        (!string.IsNullOrEmpty(pedido.Observaciones) && pedido.Observaciones.Contains("[CIRUGIA_ADHOC:"));

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

                        decimal cantidadADespachar = detalle.CantidadSolicitada;
                        if (cantidadesAprobadas != null && cantidadesAprobadas.TryGetValue(detalle.Id, out var cap))
                        {
                            cantidadADespachar = cap;
                        }

                        if (cantidadADespachar < 0) cantidadADespachar = 0;

                        // Regla de observación obligatoria cuando cantidad enviada < cantidad solicitada
                        if (cantidadADespachar < detalle.CantidadSolicitada)
                        {
                            string? obs = null;
                            observacionesPorDetalle?.TryGetValue(detalle.Id, out obs);

                            if (string.IsNullOrWhiteSpace(obs))
                            {
                                throw new InvalidOperationException($"Debe justificar mediante una observación el ajuste de cantidad enviada para el ítem '{detalle.Insumo.Nombre}' (Solicitada: {detalle.CantidadSolicitada}, Aenviar: {cantidadADespachar}).");
                            }

                            detalle.SetObservacionDespacho(obs);
                        }
                        else if (observacionesPorDetalle != null && observacionesPorDetalle.TryGetValue(detalle.Id, out var obsOpcional))
                        {
                            detalle.SetObservacionDespacho(obsOpcional);
                        }

                        var stockActual = stockSede.StockActual;
                        if (stockActual < cantidadADespachar)
                        {
                            throw new InvalidOperationException($"Stock insuficiente de '{detalle.Insumo.Nombre}' en la sede proveedora. Aprobado: {cantidadADespachar}, Disponible: {stockActual}");
                        }

                        if (cantidadADespachar > 0)
                        {
                            // 1. Descuenta stock de Sede Proveedora (Almacén Principal)
                            stockSede.RegistrarMovimientoStock(-cantidadADespachar, detalle.Insumo.PermiteFraccionamiento);
                            _logger.LogInformation("Stock transferido desde Sede Proveedora {SedeId}. Insumo: {InsumoId}, Cantidad: {Cantidad}", pedido.SedeProveedoraId, detalle.InsumoId, cantidadADespachar);
                            detalle.SetDespachado(cantidadADespachar);
                            detalle.SetRecibido(cantidadADespachar);

                            // Registrar movimiento de salida en Sede Proveedora
                            var tipoMovSalida = esGastoInterno ? "ConsumoInterno" : (esCirugiaAdHoc ? "DespachoQuirofano" : "TransferenciaSalida");
                            var motivoSalidaTxt = esGastoInterno 
                                ? $"Nota de Entrega por Consumo Interno de Laboratorio/Mantenimiento ({pedido.Correlativo})"
                                : (esCirugiaAdHoc 
                                    ? $"Despacho de urgencia hacia Quirófano ({pedido.Correlativo}): {detalle.Insumo.Nombre}" 
                                    : $"Despacho de pedido inter-sede {pedido.Correlativo} hacia sede solicitante (Cant. Aprobada: {cantidadADespachar})");

                            var movimientoSalida = new MovimientoInsumo(
                                detalle.InsumoId,
                                pedido.SedeProveedoraId,
                                TipoMovimientoInsumo.TransferenciaSalida,
                                -cantidadADespachar,
                                (UnidadMedidaEnum)detalle.Insumo.UnidadMedidaId,
                                cantidadADespachar,
                                motivoSalidaTxt,
                                usuarioId
                            );
                            _context.MovimientosInsumo.Add(movimientoSalida);

                            // 2. Si no es Gasto Interno (Consumo Inmediato de Lab/Mantenimiento), sumar inmediatamente el stock a la Sede Solicitante (incluyendo Cirugía / Quirófano)
                            if (!esGastoInterno)
                            {
                                var stockSolicitante = await _context.StocksSedes
                                    .FirstOrDefaultAsync(s => s.InsumoId == detalle.InsumoId && s.SedeId == pedido.SedeSolicitanteId, cancellationToken);

                                if (stockSolicitante == null)
                                {
                                    stockSolicitante = new StockSede(detalle.InsumoId, pedido.SedeSolicitanteId, 0);
                                    _context.StocksSedes.Add(stockSolicitante);
                                }

                                stockSolicitante.RegistrarMovimientoStock(cantidadADespachar, detalle.Insumo.PermiteFraccionamiento);
                                _logger.LogInformation("Stock recibido automáticamente en Sede Solicitante {SedeId}. Insumo: {InsumoId}, Cantidad: {Cantidad}", pedido.SedeSolicitanteId, detalle.InsumoId, cantidadADespachar);

                                var movimientoEntrada = new MovimientoInsumo(
                                    detalle.InsumoId,
                                    pedido.SedeSolicitanteId,
                                    TipoMovimientoInsumo.TransferenciaEntrada,
                                    cantidadADespachar,
                                    (UnidadMedidaEnum)detalle.Insumo.UnidadMedidaId,
                                    cantidadADespachar,
                                    $"Recepción por despacho de pedido inter-sede {pedido.Correlativo}",
                                    usuarioId

                                );
                                _context.MovimientosInsumo.Add(movimientoEntrada);
                            }

                            // 3. Si viene asociado a una Solicitud Ad-Hoc de Cirugía, marcar la solicitud como despachada y registrar en log
                            if (esCirugiaAdHoc)
                            {
                                if (!string.IsNullOrEmpty(pedido.Observaciones) && pedido.Observaciones.Contains("[CIRUGIA_ADHOC:"))
                                {
                                    var startIdx = pedido.Observaciones.IndexOf("[CIRUGIA_ADHOC:") + 15;
                                    var endIdx = pedido.Observaciones.IndexOf(':', startIdx);
                                    var ordenEndIdx = pedido.Observaciones.IndexOf(']', endIdx);
                                    if (endIdx > startIdx && ordenEndIdx > endIdx)
                                    {
                                        var solIdStr = pedido.Observaciones.Substring(startIdx, endIdx - startIdx);
                                        var ordIdStr = pedido.Observaciones.Substring(endIdx + 1, ordenEndIdx - endIdx - 1);
                                        if (Guid.TryParse(solIdStr, out var solId) && Guid.TryParse(ordIdStr, out var ordId))
                                        {
                                            var sol = await _context.SolicitudesInsumosCirugia
                                                .Include(s => s.OrdenCirugia)
                                                .FirstOrDefaultAsync(s => s.Id == solId, cancellationToken);

                                            if (sol != null && sol.EstadoSolicitud == SistemaSatHospitalario.Core.Domain.Entities.Admision.EstadoSolicitudInsumoConstants.Pendiente)
                                            {
                                                var log = new SistemaSatHospitalario.Core.Domain.Entities.Admision.CirugiaLog(ordId, usuarioId, SistemaSatHospitalario.Core.Domain.Entities.Admision.CirugiaEventoConstants.DespachoInsumos,
                                                    $"Despachado pedido {pedido.Correlativo} desde Almacén Central: {cantidadADespachar} {detalle.Insumo.UnidadMedidaNav.Nombre} de '{detalle.Insumo.Nombre}'.");
                                                _context.CirugiaLogs.Add(log);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    pedido.CambiarEstado(EstadoPedidoInterSedeConstants.Recibido);

                    await _context.SaveChangesAsync(cancellationToken);
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retries++;
                    if (retries >= maxRetries)
                    {
                        _logger.LogError(ex, "Excedido el número máximo de reintentos por concurrencia al despachar pedido {PedidoId}", pedidoId);
                        throw;
                    }
                    _logger.LogWarning("Conflicto de concurrencia al despachar pedido {PedidoId}. Reintento {Retry}/{MaxRetries}", pedidoId, retries, maxRetries);
                    await Task.Delay(100 * retries, cancellationToken);
                }
            }
        }

        public async Task RejectPedidoAsync(
            Guid pedidoId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            string motivo,
            CancellationToken cancellationToken = default)
        {
            var pedido = await _context.PedidosInterSede
                .FirstOrDefaultAsync(p => p.Id == pedidoId, cancellationToken);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"No se encontró el pedido inter-sede con ID {pedidoId}");
            }

            if (pedido.Estado != EstadoPedidoInterSedeConstants.Solicitado)
            {
                throw new InvalidOperationException("Solo se pueden rechazar pedidos que se encuentren en estado Solicitado.");
            }

            pedido.CambiarEstado(EstadoPedidoInterSedeConstants.Rechazado);
            pedido.SetObservaciones($"{pedido.Observaciones} | [RECHAZADO por {usuarioId}: {motivo}]".Trim());

            // Si es pedido quirúrgico ad-hoc, sincronizar rechazo en SolicitudInsumoCirugia
            if (!string.IsNullOrEmpty(pedido.Observaciones) && pedido.Observaciones.Contains("[CIRUGIA_ADHOC:"))
            {
                try
                {
                    var startIdx = pedido.Observaciones.IndexOf("[CIRUGIA_ADHOC:") + 15;
                    var endIdx = pedido.Observaciones.IndexOf(':', startIdx);
                    if (endIdx > startIdx)
                    {
                        var solIdStr = pedido.Observaciones.Substring(startIdx, endIdx - startIdx);
                        if (Guid.TryParse(solIdStr, out var solId))
                        {
                            var sol = await _context.SolicitudesInsumosCirugia.FirstOrDefaultAsync(s => s.Id == solId, cancellationToken);
                            if (sol != null && sol.EstadoSolicitud == SistemaSatHospitalario.Core.Domain.Entities.Admision.EstadoSolicitudInsumoConstants.Pendiente)
                            {
                                var log = new SistemaSatHospitalario.Core.Domain.Entities.Admision.CirugiaLog(sol.OrdenCirugiaId, usuarioId, "RechazoInsumoExtra",
                                    $"Solicitud ad-hoc rechazada por Almacén Central: {motivo}");
                                _context.CirugiaLogs.Add(log);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al sincronizar rechazo de solicitud de cirugía");
                }
            }

            _logger.LogInformation("Pedido inter-sede {Correlativo} rechazada por {UsuarioId}. Motivo: {Motivo}", pedido.Correlativo, usuarioId, motivo);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ReceivePedidoAsync(
            Guid pedidoId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Dictionary<Guid, decimal> discrepancias,
            CancellationToken cancellationToken = default)
        {
            var pedido = await _context.PedidosInterSede
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Insumo)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, cancellationToken);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"No se encontró el pedido inter-sede con ID {pedidoId}");
            }

            if (pedido.Estado != EstadoPedidoInterSedeConstants.Despachado)
            {
                throw new InvalidOperationException("El pedido no está en un estado que permita recepción.");
            }

            foreach (var detalle in pedido.Detalles)
            {
                decimal cantidadRecibida = detalle.CantidadDespachada;
                if (discrepancias != null && discrepancias.TryGetValue(detalle.InsumoId, out decimal cantDiscrepancia))
                {
                    cantidadRecibida = cantDiscrepancia;
                }

                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == detalle.InsumoId && s.SedeId == pedido.SedeSolicitanteId, cancellationToken);

                if (stockSede == null)
                {
                    stockSede = new StockSede(detalle.InsumoId, pedido.SedeSolicitanteId, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                stockSede.RegistrarMovimientoStock(cantidadRecibida, detalle.Insumo.PermiteFraccionamiento);
                detalle.SetRecibido(cantidadRecibida);

                var movimiento = new MovimientoInsumo(
                    detalle.InsumoId,
                    pedido.SedeSolicitanteId,
                    TipoMovimientoInsumo.TransferenciaEntrada,
                    cantidadRecibida,
                    (UnidadMedidaEnum)detalle.Insumo.UnidadMedidaId,
                    cantidadRecibida,
                    $"Recepción de pedido inter-sede {pedido.Correlativo}",
                     usuarioId
                );
                _context.MovimientosInsumo.Add(movimiento);
            }

            pedido.CambiarEstado(EstadoPedidoInterSedeConstants.Recibido);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public static decimal GetConversionFactor(UnidadMedidaEnum origen, UnidadMedidaEnum destino)
        {
            if (origen == destino) return 1.0m;

            if (origen == UnidadMedidaEnum.L && destino == UnidadMedidaEnum.ML) return 1000m;
            if (origen == UnidadMedidaEnum.ML && destino == UnidadMedidaEnum.L) return 0.001m;

            decimal origenEnGramos = origen switch
            {
                UnidadMedidaEnum.KG => 1000m,
                UnidadMedidaEnum.G => 1m,
                UnidadMedidaEnum.DG => 0.1m,
                UnidadMedidaEnum.MG => 0.001m,
                _ => 1m
            };

            decimal gramosADestino = destino switch
            {
                UnidadMedidaEnum.KG => 0.001m,
                UnidadMedidaEnum.G => 1m,
                UnidadMedidaEnum.DG => 10m,
                UnidadMedidaEnum.MG => 1000m,
                _ => 1m
            };

            bool esMasaOrigen = origen == UnidadMedidaEnum.KG || origen == UnidadMedidaEnum.G || origen == UnidadMedidaEnum.DG || origen == UnidadMedidaEnum.MG;
            bool esMasaDestino = destino == UnidadMedidaEnum.KG || destino == UnidadMedidaEnum.G || destino == UnidadMedidaEnum.DG || destino == UnidadMedidaEnum.MG;

            if (esMasaOrigen && esMasaDestino)
            {
                return origenEnGramos * gramosADestino;
            }

            return 1.0m;
        }

        public async Task RecordDiscardAsync(Guid insumoId, decimal cantidad, string motivo, Guid? usuarioId, Guid? sedeId = null, CancellationToken cancellationToken = default)
        {
             if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a descartar debe ser mayor a cero.", nameof(cantidad));
            }
            if (string.IsNullOrWhiteSpace(motivo))
            {
                throw new ArgumentException("El motivo de descarte es obligatorio para auditoría.", nameof(motivo));
            }

            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == insumoId, cancellationToken);
            if (insumo == null)
            {
                throw new KeyNotFoundException($"No se encontró el insumo con ID {insumoId}");
            }

            var targetSedeId = (sedeId.HasValue && sedeId.Value != Guid.Empty) 
                ? sedeId.Value 
                : SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal;

            var stockSede = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == insumoId && s.SedeId == targetSedeId, cancellationToken);

            if (stockSede == null || stockSede.StockActual < cantidad)
            {
                var disponible = stockSede?.StockActual ?? 0;
                throw new InvalidOperationException($"Stock insuficiente en el área/sede seleccionada para descarte. Solicitado: {cantidad}, Disponible: {disponible}");
            }

            stockSede.RegistrarMovimientoStock(-cantidad, insumo.PermiteFraccionamiento);

            var movimiento = new MovimientoInsumo(
                insumoId,
                targetSedeId,
                TipoMovimientoInsumo.Descarte,
                -cantidad,
                (UnidadMedidaEnum)insumo.UnidadMedidaId,
                cantidad,
                motivo.Trim(),
                usuarioId,
                null
            );

            _context.MovimientosInsumo.Add(movimiento);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
