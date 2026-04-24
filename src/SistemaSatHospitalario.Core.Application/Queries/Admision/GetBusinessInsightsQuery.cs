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
                response.VentasPorEspecialidad = await _context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                    .SelectMany(c => c.Detalles)
                    .GroupBy(d => d.TipoServicio)
                    .Select(g => new RevenueBySpecialtyDto
                    {
                        Especialidad = g.Key,
                        Monto = g.Sum(x => x.Precio * x.Cantidad)
                    })
                    .OrderByDescending(x => x.Monto)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                // Ventas por Seguro (Top 5 hoy)
                response.VentasPorSeguro = await (from c in _context.CuentasServicios.AsNoTracking()
                                                   join s in _context.SegurosConvenios.AsNoTracking() on c.ConvenioId equals (int?)s.Id into joinSeg
                                                   from s in joinSeg.DefaultIfEmpty()
                                                   where c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada
                                                   group c by s != null ? s.Nombre : EstadoConstants.Particular into g
                                                   select new RevenueByInsuranceDto
                                                   {
                                                       Seguro = g.Key,
                                                       Monto = g.SelectMany(x => x.Detalles).Sum(d => d.Precio * d.Cantidad)
                                                   })
                                                   .OrderByDescending(x => x.Monto)
                                                   .Take(5)
                                                   .ToListAsync(cancellationToken);

                // --- ANALYTICS ENRICHMENT (Fase 6) ---

                // 1. Tendencia de Ingresos (Últimos 7 días) - Senior Refactor: In-memory grouping for TZ resilience
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
                        var amount = rawPayments
                            .Where(p => p.FechaPago.AddHours(-4).Date == hospitalDate)
                            .Sum(p => p.EquivalenteAbonadoBase);
                        
                        _logger.LogDebug("[INSIGHTS] Tendencia {Date}: {Amount}", hospitalDate.ToString("dd/MM"), amount);
                        
                        return new RevenueTrendDto
                        {
                            Fecha = hospitalDate.ToString("dd/MM"),
                            Monto = amount
                        };
                    })
                    .ToList();

                // 2. Distribución de Pacientes (Particular vs Seguros)
                var totalPatients = await _context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                    .CountAsync(cancellationToken);

                if (totalPatients > 0)
                {
                    var particularCount = await _context.CuentasServicios
                        .AsNoTracking()
                        .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada && c.ConvenioId == null)
                        .CountAsync(cancellationToken);

                    var insuranceCount = totalPatients - particularCount;

                    response.DistribucionPacientes = new List<PatientDistributionDto>
                    {
                        new PatientDistributionDto { Etiqueta = "Particular", Valor = particularCount },
                        new PatientDistributionDto { Etiqueta = "Convenios/Seguros", Valor = insuranceCount }
                    };
                }

                // 3. --- PROBUG DETECTION (Fase 6) ---
                
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

            return response;
        }
    }
}
