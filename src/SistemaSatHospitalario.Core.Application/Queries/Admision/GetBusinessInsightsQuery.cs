using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetBusinessInsightsQuery : IRequest<BusinessInsightsDto>
    {
        public string? UserRole { get; set; }
    }

    public class GetBusinessInsightsQueryHandler : IRequestHandler<GetBusinessInsightsQuery, BusinessInsightsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetBusinessInsightsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessInsightsDto> Handle(GetBusinessInsightsQuery request, CancellationToken cancellationToken)
        {
            // [SENIOR NOTE] Usamos la zona horaria del hospital (UTC-4) para calcular "Hoy"
            // Esto asegura que los cierres de caja y recaudación coincidan con el día operativo real.
            var hospitalNow = DateTime.UtcNow.AddHours(-4);
            var today = hospitalNow.Date;
            var tomorrow = today.AddDays(1);
            
            // Convertimos de vuelta a UTC para filtrar en la base de datos (si las fechas se guardan en UTC)
            var todayUtc = today.AddHours(4);
            var tomorrowUtc = tomorrow.AddHours(4);
            var response = new BusinessInsightsDto();

            // 1. Métricas de Caja/Admisión (Admin o Cajeros)
            if (AuthorizationConstants.IsCajero(request.UserRole))
            {
                // Ventas Netas hoy (Facturado hoy) - Senior Correction: Usamos FechaPago real de los detalles
                response.TotalVentasHoy = await _context.DetallesPago
                    .AsNoTracking()
                    .Where(d => d.FechaPago >= todayUtc && d.FechaPago < tomorrowUtc && d.ReciboFactura.EstadoFiscal != EstadoConstants.Anulada)
                    .SumAsync(d => d.EquivalenteAbonadoBase, cancellationToken);

                // Pacientes Atendidos -> Refactor: Mostramos INGRESOS de hoy para reflejar actividad real
                // Anteriormente solo mostraba Cuentas de hoy facturadas (ocultaba ingresos nuevos)
                response.PacientesAtendidosHoy = await _context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != EstadoConstants.Anulada)
                    .CountAsync(cancellationToken);

                // Turnos Pautados Hoy
                response.TurnosPautadosHoy = await _context.CitasMedicas
                    .AsNoTracking()
                    .Where(c => c.HoraPautada >= todayUtc && c.HoraPautada < tomorrowUtc && c.Estado != EstadoConstants.Cancelado)
                    .CountAsync(cancellationToken);
            }

            // 2. Métricas de RX (Admin o RX)
            if (AuthorizationConstants.IsLaboratorio(request.UserRole))
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
            if (AuthorizationConstants.IsAdmin(request.UserRole))
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
            }

            return response;
        }
    }
}
