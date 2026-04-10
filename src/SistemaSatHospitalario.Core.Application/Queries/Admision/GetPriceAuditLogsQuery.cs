using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPriceAuditLogsQuery : IRequest<List<PriceAuditLogDto>>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    public class PriceAuditLogDto
    {
        public Guid Id { get; set; }
        public string DescripcionServicio { get; set; }
        public decimal PrecioOriginal { get; set; }
        public decimal PrecioModificado { get; set; }
        public decimal HonorarioAnterior { get; set; }
        public decimal NuevoHonorario { get; set; }
        public decimal Varianza => PrecioModificado - PrecioOriginal;
        public decimal VarianzaPorcentual => PrecioOriginal != 0 ? (Varianza / PrecioOriginal) * 100 : 0;
        public string UsuarioOperador { get; set; }
        public string AutorizadoPor { get; set; }
        public DateTime FechaModificacion { get; set; }
    }

    public class GetPriceAuditLogsQueryHandler : IRequestHandler<GetPriceAuditLogsQuery, List<PriceAuditLogDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPriceAuditLogsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PriceAuditLogDto>> Handle(GetPriceAuditLogsQuery request, CancellationToken ct)
        {
            var query = _context.AuditLogsPrecios.AsNoTracking();

            if (request.FechaDesde.HasValue)
                query = query.Where(x => x.FechaModificacion >= request.FechaDesde.Value);

            if (request.FechaHasta.HasValue)
                query = query.Where(x => x.FechaModificacion <= request.FechaHasta.Value);

            return await query
                .OrderByDescending(x => x.FechaModificacion)
                .Select(x => new PriceAuditLogDto
                {
                    Id = x.Id,
                    DescripcionServicio = x.DescripcionServicio,
                    PrecioOriginal = x.PrecioOriginal,
                    PrecioModificado = x.PrecioModificado,
                    HonorarioAnterior = x.HonorarioAnterior,
                    NuevoHonorario = x.NuevoHonorario,
                    UsuarioOperador = x.UsuarioOperador,
                    AutorizadoPor = x.AutorizadoPor,
                    FechaModificacion = x.FechaModificacion
                })
                .ToListAsync(ct);
        }
    }
}
