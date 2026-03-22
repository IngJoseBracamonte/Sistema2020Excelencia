using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

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

            // 1. Obtener todas las cuentas creadas hoy por el usuario
            var cuentasHoy = await _context.CuentasPorCobrar
                .Where(c => c.FechaCreacion >= today && c.FechaCreacion < tomorrow)
                // Nota: Idealmente filtrar por UserId si lo tenemos en la entidad
                .ToListAsync(cancellationToken);

            // 2. Obtener todos los abonos procesados hoy
            // (Asumiendo que los abonos estan dentro de las cuentas o en una tabla aparte)
            // Por simplicidad en este MVP, agregamos los abonos que estan en memoria de las cuentas cargadas
            
            var allPayments = cuentasHoy.SelectMany(c => c.Abonos).ToList();

            var summary = new DailyClosingDto
            {
                Fecha = today,
                Usuario = request.UserId ?? "Asistente",
                TotalOrdenes = cuentasHoy.Count,
                TotalVendidoUSD = cuentasHoy.Sum(c => c.TotalCargadoBase),
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
