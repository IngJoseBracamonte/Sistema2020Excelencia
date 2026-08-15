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
    public class GetHistorialMovimientosQuery : IRequest<List<HistorialMovimientoDto>>
    {
        public string? TipoMovimiento { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class HistorialMovimientoDto
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string InsumoCodigo { get; set; } = string.Empty;
        public string InsumoNombre { get; set; } = string.Empty;
        public Guid SedeId { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal CantidadBase { get; set; }
        public decimal CantidadOriginal { get; set; }
        public string UnidadMedidaOriginal { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class GetHistorialMovimientosQueryHandler : IRequestHandler<GetHistorialMovimientosQuery, List<HistorialMovimientoDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetHistorialMovimientosQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistorialMovimientoDto>> Handle(GetHistorialMovimientosQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MovimientosInsumo
                .Include(m => m.Insumo)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TipoMovimiento) && Enum.TryParse<SistemaSatHospitalario.Core.Domain.Enums.TipoMovimientoInsumo>(request.TipoMovimiento, true, out var parsedTipo))
            {
                query = query.Where(m => m.TipoMovimiento == parsedTipo);
            }

            if (request.FechaDesde.HasValue)
            {
                var fDesde = request.FechaDesde.Value.Date;
                query = query.Where(m => m.Fecha >= fDesde);
            }

            if (request.FechaHasta.HasValue)
            {
                var fHasta = request.FechaHasta.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(m => m.Fecha <= fHasta);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.Trim().ToLower();
                query = query.Where(m =>
                    (m.Insumo != null && m.Insumo.Nombre != null && m.Insumo.Nombre.ToLower().Contains(searchLower)) ||
                    (m.Insumo != null && m.Insumo.Codigo != null && m.Insumo.Codigo.ToLower().Contains(searchLower)) ||
                    (m.Usuario != null && m.Usuario.ToLower().Contains(searchLower)) ||
                    (m.Motivo != null && m.Motivo.ToLower().Contains(searchLower))
                );
            }

            var items = await query
                .OrderByDescending(m => m.Fecha)
                .Select(m => new HistorialMovimientoDto
                {
                    Id = m.Id,
                    InsumoId = m.InsumoId,
                    InsumoCodigo = m.Insumo != null ? m.Insumo.Codigo : "N/A",
                    InsumoNombre = m.Insumo != null ? m.Insumo.Nombre : "Insumo Eliminado",
                    SedeId = m.SedeId,
                    TipoMovimiento = m.TipoMovimiento.ToString(),
                    CantidadBase = m.CantidadBase,
                    CantidadOriginal = m.CantidadOriginal,
                    UnidadMedidaOriginal = m.UnidadMedidaOriginal.ToString(),
                    Usuario = m.Usuario ?? "Sistema",
                    Fecha = m.Fecha,
                    Motivo = m.Motivo ?? ""
                })
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
