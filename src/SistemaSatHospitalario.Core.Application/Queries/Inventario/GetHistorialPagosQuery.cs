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
    public class GetHistorialPagosQuery : IRequest<List<PagoProveedorDto>>
    {
        public string? Busqueda { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    public class GetHistorialPagosQueryHandler : IRequestHandler<GetHistorialPagosQuery, List<PagoProveedorDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetHistorialPagosQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PagoProveedorDto>> Handle(GetHistorialPagosQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PagosProveedores
                .AsNoTracking()
                .Include(p => p.OrdenCompra)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Busqueda))
            {
                var term = request.Busqueda.Trim().ToLower();
                query = query.Where(p => 
                    p.OrdenCompra.ProveedorNombre.ToLower().Contains(term) || 
                    p.OrdenCompra.NumeroFactura.ToLower().Contains(term) ||
                    p.Referencia.ToLower().Contains(term) ||
                    p.UsuarioId.ToLower().Contains(term)
                );
            }

            if (request.FechaDesde.HasValue)
            {
                query = query.Where(p => p.FechaPago >= request.FechaDesde.Value);
            }

            if (request.FechaHasta.HasValue)
            {
                query = query.Where(p => p.FechaPago <= request.FechaHasta.Value);
            }

            var pagos = await query
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync(cancellationToken);

            return pagos.Select(p => new PagoProveedorDto
            {
                Id = p.Id,
                OrdenCompraId = p.OrdenCompraId,
                NumeroFactura = p.OrdenCompra.NumeroFactura,
                ProveedorNombre = p.OrdenCompra.ProveedorNombre,
                FechaPago = p.FechaPago,
                MontoAbonadoUSD = p.MontoAbonadoUSD,
                TasaCambio = p.TasaCambio,
                MontoAbonadoBs = p.MontoAbonadoBs,
                MetodoPago = p.MetodoPago,
                Referencia = p.Referencia,
                UsuarioId = p.UsuarioId,
                Observaciones = p.Observaciones
            }).ToList();
        }
    }
}
