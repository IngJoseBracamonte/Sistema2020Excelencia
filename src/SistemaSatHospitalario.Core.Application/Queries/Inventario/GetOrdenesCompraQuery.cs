using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Inventario;

namespace SistemaSatHospitalario.Core.Application.Queries.Inventario
{
    public class GetOrdenesCompraQuery : IRequest<List<OrdenCompraInventarioDto>>
    {
        public string? Estado { get; set; } // PorPagar, Pagado, Todos
        public string? Busqueda { get; set; } // Proveedor o NumeroFactura
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    public class GetOrdenesCompraQueryHandler : IRequestHandler<GetOrdenesCompraQuery, List<OrdenCompraInventarioDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdenesCompraQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrdenCompraInventarioDto>> Handle(GetOrdenesCompraQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrdenesCompraInventario
                .AsNoTracking()
                .Include(o => o.Pagos)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Estado) && !request.Estado.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => o.Estado.ToLower() == request.Estado.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(request.Busqueda))
            {
                var term = request.Busqueda.Trim().ToLower();
                query = query.Where(o => o.ProveedorNombre.ToLower().Contains(term) || o.NumeroFactura.ToLower().Contains(term));
            }

            if (request.FechaDesde.HasValue)
            {
                query = query.Where(o => o.FechaEmision >= request.FechaDesde.Value);
            }

            if (request.FechaHasta.HasValue)
            {
                query = query.Where(o => o.FechaEmision <= request.FechaHasta.Value);
            }

            var ordenes = await query
                .OrderByDescending(o => o.FechaEmision)
                .ToListAsync(cancellationToken);

            return ordenes.Select(o => new OrdenCompraInventarioDto
            {
                Id = o.Id,
                NumeroFactura = o.NumeroFactura,
                ProveedorId = o.ProveedorId,
                ProveedorNombre = o.ProveedorNombre,
                FechaEmision = o.FechaEmision,
                MontoTotalUSD = o.MontoTotalUSD,
                MontoTotalBs = o.MontoTotalBs,
                TotalAbonadoUSD = o.TotalAbonadoUSD,
                SaldoPendienteUSD = o.SaldoPendienteUSD,
                Estado = o.Estado,
                Observaciones = o.Observaciones,
                Pagos = o.Pagos.Select(p => new PagoProveedorDto
                {
                    Id = p.Id,
                    OrdenCompraId = o.Id,
                    NumeroFactura = o.NumeroFactura,
                    ProveedorNombre = o.ProveedorNombre,
                    FechaPago = p.FechaPago,
                    MontoAbonadoUSD = p.MontoAbonadoUSD,
                    TasaCambio = p.TasaCambio,
                    MontoAbonadoBs = p.MontoAbonadoBs,
                    MetodoPago = p.MetodoPago,
                    Referencia = p.Referencia,
                    UsuarioId = p.UsuarioId,
                    Observaciones = p.Observaciones
                }).OrderByDescending(p => p.FechaPago).ToList()
            }).ToList();
        }
    }
}
