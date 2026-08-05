using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPedidosInterSedeHistorialQueryHandler : IRequestHandler<GetPedidosInterSedeHistorialQuery, List<PedidoInterSedeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPedidosInterSedeHistorialQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PedidoInterSedeDto>> Handle(GetPedidosInterSedeHistorialQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PedidosInterSede
                .IgnoreQueryFilters()
                .Include(p => p.SedeSolicitante)
                .Include(p => p.SedeProveedora)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Insumo)
                .OrderByDescending(p => p.FechaCreacion);

            var items = await query.ToListAsync(cancellationToken);

            return items.Select(p => new PedidoInterSedeDto
            {
                Id = p.Id,
                Correlativo = p.Correlativo,
                SedeSolicitanteId = p.SedeSolicitanteId,
                SedeSolicitanteNombre = p.SedeSolicitante != null ? p.SedeSolicitante.Nombre : "Sede Desconocida",
                SedeProveedoraId = p.SedeProveedoraId,
                SedeProveedoraNombre = p.SedeProveedora != null ? p.SedeProveedora.Nombre : "Almacén Principal",
                Estado = p.Estado.ToString(),
                FechaCreacion = p.FechaCreacion,
                FechaDespacho = p.FechaDespacho,
                FechaRecepcion = p.FechaRecepcion,
                UsuarioCreador = p.UsuarioCreador,
                Observaciones = p.Observaciones,
                Detalles = p.Detalles.Select(d => new PedidoInterSedeDetalleDto
                {
                    Id = d.Id,
                    InsumoId = d.InsumoId,
                    InsumoNombre = d.Insumo != null ? d.Insumo.Nombre : "Insumo Desactivado",
                    InsumoCodigo = d.Insumo != null ? d.Insumo.Codigo : "N/A",
                    CantidadSolicitada = d.CantidadSolicitada,
                    CantidadDespachada = d.CantidadDespachada,
                    CantidadRecibida = d.CantidadRecibida
                }).ToList()
            }).ToList();
        }
    }
}
