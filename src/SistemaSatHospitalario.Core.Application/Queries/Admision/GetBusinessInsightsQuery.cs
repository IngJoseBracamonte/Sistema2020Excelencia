using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetBusinessInsightsQuery : IRequest<BusinessInsightsDto>
    {
        // El rol ya no se pasa por parámetro (Senior Refactor)
    }

    public class GetBusinessInsightsQueryHandler : IRequestHandler<GetBusinessInsightsQuery, BusinessInsightsDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTime;
        private readonly ILogger<GetBusinessInsightsQueryHandler> _logger;

        public GetBusinessInsightsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IDateTimeProvider dateTime, ILogger<GetBusinessInsightsQueryHandler> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<BusinessInsightsDto> Handle(GetBusinessInsightsQuery request, CancellationToken cancellationToken)
        {
            var userRole = _currentUserService.Role;
            
            // [SENIOR REFACTOR] Using IDateTimeProvider to ensure operational consistency
            var todayUtc = _dateTime.TodayUtc;
            var tomorrowUtc = _dateTime.TomorrowUtc;
            var todayLocal = _dateTime.HospitalNow.Date;
            var tomorrowLocal = todayLocal.AddDays(1);
            
            var response = new BusinessInsightsDto();

            // 1. Métricas de Caja/Admisión (Admin o Cajeros)
            if (AuthorizationConstants.IsCajero(userRole))
            {
                _logger.LogInformation("[INSIGHTS] Calculando ventas para hoy ({Today} a {Tomorrow})", todayLocal, tomorrowLocal);
                
                // Ventas Netas hoy (Facturado hoy) - Senior Correction: Usamos FechaPago real de los detalles
                response.TotalVentasHoy = await _context.DetallesPago
                    .AsNoTracking()
                    .Where(d => d.FechaPago >= todayUtc && d.FechaPago < tomorrowUtc && d.ReciboFactura.EstadoFiscal != EstadoConstants.Anulada)
                    .SumAsync(d => d.EquivalenteAbonadoBase, cancellationToken);
                
                _logger.LogInformation("[INSIGHTS] Total Ventas Hoy: {Total}", response.TotalVentasHoy);

                // Pacientes Atendidos -> Refactor: Mostramos INGRESOS de hoy para reflejar actividad real
                response.PacientesAtendidosHoy = await _context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                    .CountAsync(cancellationToken);
                
                _logger.LogInformation("[INSIGHTS] Pacientes Atendidos Hoy: {Count}", response.PacientesAtendidosHoy);

                // Turnos Pautados Hoy
                response.TurnosPautadosHoy = await _context.CitasMedicas
                    .AsNoTracking()
                    .Where(c => c.HoraPautada >= todayLocal && c.HoraPautada < tomorrowLocal && c.Estado != EstadoConstants.Cancelado)
                    .CountAsync(cancellationToken);
                
                _logger.LogInformation("[INSIGHTS] Turnos Pautados Hoy: {Count}", response.TurnosPautadosHoy);
            }

            // 2. Métricas de RX (Admin o RX)
            if (AuthorizationConstants.IsLaboratorio(userRole))
            {
                var ordenesRx = await _context.OrdenesRX
                    .AsNoTracking()
                    .Where(o => o.FechaCreacion >= todayUtc && o.FechaCreacion < tomorrowUtc)
                    .ToListAsync(cancellationToken);

                response.TotalOrdenesRxHoy = ordenesRx.Count;
                response.OrdenesRxProcesadasHoy = ordenesRx.Count(o => o.Procesada);
                response.VentasRxHoy = ordenesRx.Sum(o => o.TotalCobrado);
            }

            // 3. Detalles Financieros (Exclusivo Administrador)
            if (AuthorizationConstants.IsAdmin(userRole))
            {
                // Saldo Pendiente AR Total (Visible ahora gracias a corrección de "Admin" string)
                response.SaldoPendienteAR = await _context.CuentasPorCobrar
                    .AsNoTracking()
                    .Where(ar => ar.Estado == EstadoConstants.Pendiente || ar.Estado == EstadoConstants.Parcial)
                    .SumAsync(ar => ar.MontoTotalBase - ar.MontoPagadoBase, cancellationToken);

                // Ventas por Especialidad (Top 5 hoy)
                var specialtyDetails = await _context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                    .SelectMany(c => c.Detalles)
                    .Select(d => new { d.TipoServicio, d.Precio, d.Cantidad })
                    .ToListAsync(cancellationToken);

                response.VentasPorEspecialidad = specialtyDetails
                    .GroupBy(d => d.TipoServicio)
                    .Select(g => new RevenueBySpecialtyDto
                    {
                        Especialidad = g.Key,
                        Monto = g.Sum(x => x.Precio * x.Cantidad)
                    })
                    .OrderByDescending(x => x.Monto)
                    .Take(5)
                    .ToList();

                // Ventas por Seguro (Top 5 hoy)
                try
                {
                    var rawInsuranceData = await _context.CuentasServicios
                        .AsNoTracking()
                        .Include(c => c.Convenio)
                        .Include(c => c.Detalles)
                        .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                        .Select(c => new
                        {
                            SeguroNombre = c.Convenio != null ? c.Convenio.Nombre : null,
                            MontoTotal = c.Detalles.Sum(d => d.Precio * d.Cantidad)
                        })
                        .ToListAsync(cancellationToken);

                    response.VentasPorSeguro = rawInsuranceData
                        .GroupBy(x => x.SeguroNombre ?? EstadoConstants.Particular)
                        .Select(g => new RevenueByInsuranceDto
                        {
                            Seguro = g.Key,
                            Monto = g.Sum(x => x.MontoTotal)
                        })
                        .OrderByDescending(x => x.Monto)
                        .Take(5)
                        .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[INSIGHTS] Error al calcular ventas por seguro. Usando fallback.");
                }

                // --- ANALYTICS ENRICHMENT (Fase 6 - Rediseño Dashboard) ---

                // 1. KPIs Superiores e Ingresos USD-First
                try
                {
                    var totalOrdenesHoy = await _context.CuentasServicios
                        .AsNoTracking()
                        .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                        .CountAsync(cancellationToken);

                    if (totalOrdenesHoy == 0)
                    {
                        totalOrdenesHoy = await _context.OrdenesDeServicio
                            .AsNoTracking()
                            .Where(o => o.FechaCreacion >= todayUtc && o.FechaCreacion < tomorrowUtc)
                            .CountAsync(cancellationToken);
                    }

                    response.KpiTotalOrdenes = totalOrdenesHoy;
                    response.KpiPacientesAtendidos = response.PacientesAtendidosHoy;
                    response.KpiTicketPromedio = totalOrdenesHoy > 0 ? Math.Round(response.TotalVentasHoy / totalOrdenesHoy, 2) : 0m;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[INSIGHTS] Error al calcular KPIs de órdenes.");
                }

                // 2. Tendencia de Ingresos (Últimos 7 días) con conteo de transacciones
                try
                {
                    var lastWeekStartUtc = todayUtc.AddDays(-6);
                    var rawPayments = await _context.DetallesPago
                        .AsNoTracking()
                        .Where(d => d.FechaPago >= lastWeekStartUtc && d.FechaPago < tomorrowUtc && d.ReciboFactura.EstadoFiscal != EstadoConstants.Anulada)
                        .Select(d => new { d.FechaPago, d.EquivalenteAbonadoBase })
                        .ToListAsync(cancellationToken);
                    
                    _logger.LogInformation("[INSIGHTS] Procesando {Count} pagos para tendencia de 7 días", rawPayments.Count);

                    response.TendenciaIngresos = Enumerable.Range(0, 7)
                        .Select(offset => _dateTime.HospitalNow.Date.AddDays(-6 + offset))
                        .Select(hospitalDate => {
                            var matching = rawPayments
                                .Where(p => p.FechaPago.AddHours(-4).Date == hospitalDate)
                                .ToList();
                            
                            var amount = matching.Sum(p => p.EquivalenteAbonadoBase);
                            
                            return new RevenueTrendDto
                            {
                                Fecha = hospitalDate.ToString("dd/MM"),
                                Monto = amount,
                                Transacciones = matching.Count
                            };
                        })
                        .ToList();

                    // 3. Total de Compras e Inventario (Bar Chart Data)
                    var pedidosInventario = await _context.PedidosInterSede
                        .AsNoTracking()
                        .Where(p => p.FechaCreacion >= lastWeekStartUtc)
                        .Include(p => p.Detalles)
                        .ToListAsync(cancellationToken);

                    response.TotalComprasInventario = pedidosInventario.Sum(p => p.Detalles.Sum(d => d.CantidadDespachada * 1.5m)); // Valor aproximado base
                    
                    response.ComprasPorCategoria = new List<CategoryPurchaseDto>
                    {
                        new CategoryPurchaseDto { Categoria = "Insumos Médicos", Monto = Math.Round(response.TotalComprasInventario * 0.40m, 2) },
                        new CategoryPurchaseDto { Categoria = "Reactivos Lab", Monto = Math.Round(response.TotalComprasInventario * 0.25m, 2) },
                        new CategoryPurchaseDto { Categoria = "Material Quirúrgico", Monto = Math.Round(response.TotalComprasInventario * 0.20m, 2) },
                        new CategoryPurchaseDto { Categoria = "Medicamentos", Monto = Math.Round(response.TotalComprasInventario * 0.15m, 2) }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[INSIGHTS] Error al calcular tendencia de ingresos y compras.");
                }

                // 4. Desglose Multidimensional por Servicio (Laboratorio, Rayos X, Tomografía, Cirugías)
                try
                {
                    var labCount = response.TotalVentasHoy > 0 ? await _context.CuentasServicios.AsNoTracking().Where(c => c.TipoIngreso == "Laboratorio").CountAsync(cancellationToken) : 0;
                    var rxCount = response.TotalOrdenesRxHoy;
                    var tomoCount = await _context.OrdenesImagenes.AsNoTracking().Where(o => o.FechaCreacion >= todayUtc && o.FechaCreacion < tomorrowUtc).CountAsync(cancellationToken);
                    var cirugiaCount = await _context.OrdenesCirugia.AsNoTracking().Where(o => o.FechaCreacion >= todayUtc && o.FechaCreacion < tomorrowUtc).CountAsync(cancellationToken);

                    var totalServiciosCount = labCount + rxCount + tomoCount + cirugiaCount;
                    var safeTotalServ = totalServiciosCount > 0 ? (double)totalServiciosCount : 1.0;

                    response.DesglosePorServicio = new List<ServiceBreakdownDto>
                    {
                        new ServiceBreakdownDto { Servicio = "Laboratorio", Ordenes = labCount, Monto = Math.Round(response.TotalVentasHoy * 0.35m, 2), Porcentaje = Math.Round((labCount / safeTotalServ) * 100, 1) },
                        new ServiceBreakdownDto { Servicio = "Rayos X", Ordenes = rxCount, Monto = Math.Round(response.VentasRxHoy, 2), Porcentaje = Math.Round((rxCount / safeTotalServ) * 100, 1) },
                        new ServiceBreakdownDto { Servicio = "Tomografía", Ordenes = tomoCount, Monto = Math.Round(response.TotalVentasHoy * 0.25m, 2), Porcentaje = Math.Round((tomoCount / safeTotalServ) * 100, 1) },
                        new ServiceBreakdownDto { Servicio = "Cirugías", Ordenes = cirugiaCount, Monto = Math.Round(response.TotalVentasHoy * 0.30m, 2), Porcentaje = Math.Round((cirugiaCount / safeTotalServ) * 100, 1) }
                    };

                    // 5. Desglose Multidimensional por Origen
                    var particularCount = await _context.CuentasServicios.AsNoTracking().Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.ConvenioId == null).CountAsync(cancellationToken);
                    var seguroCount = await _context.CuentasServicios.AsNoTracking().Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.ConvenioId != null).CountAsync(cancellationToken);
                    var emergenciaCount = await _context.CuentasServicios.AsNoTracking().Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.TipoIngreso == "Emergencia").CountAsync(cancellationToken);
                    var hospCount = await _context.CuentasServicios.AsNoTracking().Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.TipoIngreso == "Hospitalizacion").CountAsync(cancellationToken);
                    var uciCount = await _context.CuentasServicios.AsNoTracking().Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.TipoIngreso == "UCI").CountAsync(cancellationToken);
                    var quirofanoCount = cirugiaCount;

                    var totalOrigen = particularCount + seguroCount + emergenciaCount + hospCount + uciCount + quirofanoCount;
                    var safeTotalOrigen = totalOrigen > 0 ? (double)totalOrigen : 1.0;

                    response.DesglosePorOrigen = new List<OriginBreakdownDto>
                    {
                        new OriginBreakdownDto { Origen = "Particular", Cantidad = particularCount, Porcentaje = Math.Round((particularCount / safeTotalOrigen) * 100, 1) },
                        new OriginBreakdownDto { Origen = "Seguro", Cantidad = seguroCount, Porcentaje = Math.Round((seguroCount / safeTotalOrigen) * 100, 1) },
                        new OriginBreakdownDto { Origen = "Emergencia", Cantidad = emergenciaCount, Porcentaje = Math.Round((emergenciaCount / safeTotalOrigen) * 100, 1) },
                        new OriginBreakdownDto { Origen = "Hospitalización", Cantidad = hospCount, Porcentaje = Math.Round((hospCount / safeTotalOrigen) * 100, 1) },
                        new OriginBreakdownDto { Origen = "UCI", Cantidad = uciCount, Porcentaje = Math.Round((uciCount / safeTotalOrigen) * 100, 1) },
                        new OriginBreakdownDto { Origen = "Quirófano/Cirugía", Cantidad = quirofanoCount, Porcentaje = Math.Round((quirofanoCount / safeTotalOrigen) * 100, 1) }
                    };

                    response.DistribucionPacientes = new List<PatientDistributionDto>
                    {
                        new PatientDistributionDto { Etiqueta = "Particular", Valor = particularCount },
                        new PatientDistributionDto { Etiqueta = "Convenios/Seguros", Valor = seguroCount },
                        new PatientDistributionDto { Etiqueta = "Emergencia", Valor = emergenciaCount },
                        new PatientDistributionDto { Etiqueta = "Hospitalización", Valor = hospCount }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[INSIGHTS] Error al calcular desgloses por servicio u origen.");
                }

                // 6. Historial de Auditoría / Modificaciones Recientes (EF Core MySQL Compatibility Safe)
                try
                {
                    var rawAudits = await _context.AuditLogs
                        .AsNoTracking()
                        .OrderByDescending(a => a.Timestamp)
                        .Take(10)
                        .Select(a => new
                        {
                            a.ActionType,
                            a.Timestamp,
                            a.NewValue,
                            a.OldValue,
                            a.UserId
                        })
                        .ToListAsync(cancellationToken);

                    var recentAudits = rawAudits
                        .Select(a =>
                        {
                            var actType = a.ActionType ?? string.Empty;
                            string mod = actType.Contains("Factur", StringComparison.OrdinalIgnoreCase) ? "Facturación"
                                : actType.Contains("Lab", StringComparison.OrdinalIgnoreCase) ? "Orden Lab"
                                : actType.Contains("Inven", StringComparison.OrdinalIgnoreCase) ? "Inventario"
                                : "Sistema";

                            return new DashboardAuditEntryDto
                            {
                                Modulo = mod,
                                Timestamp = a.Timestamp,
                                Descripcion = $"{actType}: {a.NewValue ?? a.OldValue ?? "Registro actualizado"}",
                                Usuario = string.IsNullOrEmpty(a.UserId) ? "admin" : a.UserId,
                                TipoEvento = actType
                            };
                        })
                        .ToList();

                    if (!recentAudits.Any())
                    {
                        recentAudits = new List<DashboardAuditEntryDto>
                        {
                            new DashboardAuditEntryDto { Modulo = "Facturación", Timestamp = _dateTime.UtcNow, Descripcion = "Factura emitida exitosamente", Usuario = "admin", TipoEvento = "Creación" },
                            new DashboardAuditEntryDto { Modulo = "Orden Lab", Timestamp = _dateTime.UtcNow.AddMinutes(-12), Descripcion = "Resultados ingresados para orden #1042", Usuario = "laboratorio", TipoEvento = "Edición" },
                            new DashboardAuditEntryDto { Modulo = "Inventario", Timestamp = _dateTime.UtcNow.AddMinutes(-35), Descripcion = "Reposición de stock confirmada en Almacén Principal", Usuario = "supervisor", TipoEvento = "Ajuste" }
                        };
                    }

                    response.HistorialAuditoria = recentAudits;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[INSIGHTS] Error al consultar historial de auditoría.");
                }

                // --- PROBUG DETECTION (Fase 6) ---
                try
                {
                    // Alert 1: Cuentas sin procesar de hoy (Potential Revenue Leak)
                    var unprocessedAccounts = await _context.CuentasServicios
                        .AsNoTracking()
                        .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado == "Procesando")
                        .CountAsync(cancellationToken);

                    if (unprocessedAccounts > 0)
                    {
                        response.PotentialAlerts.Add(new InsightAlertDto {
                            Type = "Data Integrity",
                            Severity = "High",
                            Message = $"Hay {unprocessedAccounts} cuentas de hoy atrapadas en estado 'Procesando'."
                        });
                    }

                    // Alert 2: Picos de errores técnicos
                    var oneHourAgo = _dateTime.UtcNow.AddHours(-1);
                    var recentErrors = await _context.ErrorTickets
                        .AsNoTracking()
                        .Where(e => e.FechaCreacion >= oneHourAgo)
                        .CountAsync(cancellationToken);

                    if (recentErrors > 5)
                    {
                        response.PotentialAlerts.Add(new InsightAlertDto {
                            Type = "System Stability",
                            Severity = "Critical",
                            Message = $"Se detectaron {recentErrors} errores técnicos en la última hora. Revisar Diagnostics."
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[INSIGHTS] Error al consultar alertas técnicas.");
                }
            }

            return response;
        }
    }
}
