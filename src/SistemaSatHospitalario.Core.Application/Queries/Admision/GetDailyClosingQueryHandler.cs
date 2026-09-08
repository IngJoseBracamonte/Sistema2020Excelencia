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
        private readonly IUserResolverService _userResolver;

        public GetDailyClosingQueryHandler(IApplicationDbContext context, IUserResolverService userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<DailyClosingDto> Handle(GetDailyClosingQuery request, CancellationToken cancellationToken)
        {
            var today = request.Fecha.Date;
            var tomorrow = today.AddDays(1);

            Guid? filterUserId = null;
            if (Guid.TryParse(request.UserId, out var userIdGuid))
            {
                filterUserId = userIdGuid;
            }
            else
            {
                filterUserId = await _userResolver.ResolveUserIdByUsernameAsync(request.UserId, cancellationToken);
            }

            var cajaAbierta = await _context.CajasDiarias
                .FirstOrDefaultAsync(c => c.EstadoId == EstadoCajaConstants.AbiertaId &&
                    (filterUserId.HasValue ? c.UsuarioIdentityId == filterUserId.Value : false), cancellationToken);

            var query = _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .AsQueryable();

            if (cajaAbierta != null)
            {
                query = query.Where(r => r.CajaDiariaId == cajaAbierta.Id && r.EstadoFiscalNav.Nombre != EstadoConstants.Anulada);
            }
            else
            {
                query = query.Where(r => r.FechaEmision >= today && r.FechaEmision < tomorrow && r.EstadoFiscalNav.Nombre != EstadoConstants.Anulada);
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
