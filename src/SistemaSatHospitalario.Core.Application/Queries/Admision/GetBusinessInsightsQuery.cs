using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

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
            var today = DateTime.UtcNow.Date;
            var response = new BusinessInsightsDto();

            // 1. Métricas Globales (Solo para Administrador o Asistente Particular si se permite)
            if (request.UserRole == "Administrador" || request.UserRole == "Asistente Particular")
            {
                // Ventas Netas hoy
                response.TotalVentasHoy = await _context.RecibosFactura
                    .Where(r => r.FechaEmision.Date == today && r.Estado != "Anulado")
                    .SumAsync(r => r.DetallesPago.Sum(d => d.EquivalenteAbonadoBase), cancellationToken);

                // Pacientes atendidos
                response.PacientesAtendidosHoy = await _context.CuentasServicios
                    .Where(c => c.FechaCierre.HasValue && c.FechaCierre.Value.Date == today && c.Estado == "Facturada")
                    .CountAsync(cancellationToken);

                // Turnos Pautados Hoy
                response.TurnosPautadosHoy = await _context.CitasMedicas
                    .Where(c => c.HoraPautada.Date == today && c.Estado != "Cancelado")
                    .CountAsync(cancellationToken);
            }

            // 2. Métricas de RX (Visible para Admin y Asistente Rx)
            if (request.UserRole == "Administrador" || request.UserRole == "Asistente Rx")
            {
                var ordenesRx = await _context.OrdenesRX
                    .Where(o => o.FechaCreacion.Date == today)
                    .ToListAsync(cancellationToken);

                response.TotalOrdenesRxHoy = ordenesRx.Count;
                response.OrdenesRxProcesadasHoy = ordenesRx.Count(o => o.Procesada);
                response.VentasRxHoy = ordenesRx.Sum(o => o.TotalCobrado);
            }

            // 3. Detalles de Negocio (Exclusivo Administrador)
            if (request.UserRole == "Administrador")
            {
                // Saldo Pendiente AR Total
                response.SaldoPendienteAR = await _context.CuentasPorCobrar
                    .Where(ar => ar.Estado == "Pendiente")
                    .SumAsync(ar => ar.SaldoPendienteBase, cancellationToken);

                // Ventas por Especialidad
                response.VentasPorEspecialidad = await _context.CuentasServicios
                    .Where(c => c.FechaCierre.HasValue && c.FechaCierre.Value.Date == today && c.Estado == "Facturada")
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

                // Ventas por Seguro
                response.VentasPorSeguro = await (from c in _context.CuentasServicios
                                                   join s in _context.SegurosConvenios on c.ConvenioId equals (int?)s.Id into joinSeg
                                                   from s in joinSeg.DefaultIfEmpty()
                                                   where c.FechaCierre.HasValue && c.FechaCierre.Value.Date == today && c.Estado == "Facturada"
                                                   group c by s != null ? s.Nombre : "Particular" into g
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
