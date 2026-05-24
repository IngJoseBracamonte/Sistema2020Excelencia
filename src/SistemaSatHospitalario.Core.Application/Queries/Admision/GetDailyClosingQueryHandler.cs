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

            var cajaAbierta = await _context.CajasDiarias
                .FirstOrDefaultAsync(c => c.Estado == EstadoConstants.CajaAbierta && c.UsuarioId == request.UserId, cancellationToken);

            var query = _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .AsQueryable();

            if (cajaAbierta != null)
            {
                query = query.Where(r => r.CajaDiariaId == cajaAbierta.Id && r.EstadoFiscal != EstadoConstants.Anulada);
            }
            else
            {
                query = query.Where(r => r.FechaEmision >= today && r.FechaEmision < tomorrow && r.EstadoFiscal != EstadoConstants.Anulada);
            }

            var recibosCaja = await query.ToListAsync(cancellationToken);
            var allPayments = recibosCaja.SelectMany(r => r.DetallesPago).ToList();

            var summary = new DailyClosingDto
            {
                Fecha = today,
                Usuario = request.UserId ?? EstadoConstants.DefaultCajero,
                TotalOrdenes = recibosCaja.Count,
                TotalVendidoUSD = recibosCaja.Sum(r => r.TotalFacturadoUSD),
                TotalRecaudadoBase = allPayments.Sum(p => p.EquivalenteAbonadoBase),
                IsCajaAbierta = cajaAbierta != null,
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
