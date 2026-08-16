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
    public class TransferenciaReposicionDto
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string InsumoNombre { get; set; } = string.Empty;
        public string InsumoCodigo { get; set; } = string.Empty;
        public Guid SedeOrigenId { get; set; }
        public string SedeOrigenNombre { get; set; } = string.Empty;
        public Guid SedeDestinoId { get; set; }
        public string SedeDestinoNombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaTransferencia { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class GetReposicionesHistorialQuery : IRequest<List<TransferenciaReposicionDto>>
    {
        public Guid? SedeId { get; set; }
        public Guid? InsumoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Motivo { get; set; }
    }

    public class GetReposicionesHistorialQueryHandler : IRequestHandler<GetReposicionesHistorialQuery, List<TransferenciaReposicionDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetReposicionesHistorialQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<TransferenciaReposicionDto>> Handle(GetReposicionesHistorialQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TransferenciasReposicionStock
                .AsNoTracking()
                .Include(t => t.Insumo)
                .Include(t => t.SedeOrigen)
                .Include(t => t.SedeDestino)
                .AsQueryable();

            if (request.SedeId.HasValue && request.SedeId.Value != Guid.Empty)
            {
                query = query.Where(t => t.SedeOrigenId == request.SedeId.Value || t.SedeDestinoId == request.SedeId.Value);
            }

            if (request.InsumoId.HasValue && request.InsumoId.Value != Guid.Empty)
            {
                query = query.Where(t => t.InsumoId == request.InsumoId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Motivo))
            {
                query = query.Where(t => t.Motivo == request.Motivo.Trim());
            }

            if (request.FechaDesde.HasValue)
            {
                query = query.Where(t => t.FechaTransferencia >= request.FechaDesde.Value.Date);
            }

            if (request.FechaHasta.HasValue)
            {
                var hasta = request.FechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.FechaTransferencia <= hasta);
            }

            return await query
                .OrderByDescending(t => t.FechaTransferencia)
                .Select(t => new TransferenciaReposicionDto
                {
                    Id = t.Id,
                    InsumoId = t.InsumoId,
                    InsumoNombre = t.Insumo.Nombre,
                    InsumoCodigo = t.Insumo.Codigo,
                    SedeOrigenId = t.SedeOrigenId,
                    SedeOrigenNombre = t.SedeOrigen.Nombre,
                    SedeDestinoId = t.SedeDestinoId,
                    SedeDestinoNombre = t.SedeDestino.Nombre,
                    Cantidad = t.Cantidad,
                    Motivo = t.Motivo,
                    FechaTransferencia = t.FechaTransferencia,
                    UsuarioId = t.UsuarioId,
                    Observaciones = t.Observaciones
                })
                .ToListAsync(cancellationToken);
        }
    }
}
