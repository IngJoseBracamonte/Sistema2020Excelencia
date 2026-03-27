using System;
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
    public class GetDailyClosingQueryHandler : IRequestHandler<GetDailyClosingQuery, DailyClosingDto>
    {
        private readonly IApplicationDbContext _context;

        public GetDailyClosingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DailyClosingDto> Handle(GetDailyClosingQuery request, CancellationToken cancellationToken)
        {
            var today = request.Fecha.Date;
            var tomorrow = today.AddDays(1);

            // V11.0: Cargamos los recibos emitidos hoy para el desglose financiero real
            var recibosHoy = await _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .Where(r => r.FechaEmision >= today && r.FechaEmision < tomorrow && r.EstadoFiscal != EstadoConstants.Anulada)
                .ToListAsync(cancellationToken);

            var allPayments = recibosHoy.SelectMany(r => r.DetallesPago).ToList();

            var summary = new DailyClosingDto
            {
                Fecha = today,
                Usuario = request.UserId ?? EstadoConstants.DefaultCajero,
                TotalOrdenes = recibosHoy.Count,
                TotalVendidoUSD = recibosHoy.Sum(r => r.TotalFacturadoUSD),
                TotalRecaudadoBase = allPayments.Sum(p => p.EquivalenteAbonadoBase),
                DesgloseMetodos = allPayments
                    .GroupBy(p => p.MetodoPago)
                    .Select(g => new PaymentMethodSummaryDto
                    {
                        Metodo = g.Key,
                        MontoMonedaOriginal = g.Sum(p => p.MontoAbonadoMoneda),
                        MontoEquivalenteBase = g.Sum(p => p.EquivalenteAbonadoBase),
                        Conteo = g.Count()
                    }).ToList()
            };

            return summary;
        }
    }
}
