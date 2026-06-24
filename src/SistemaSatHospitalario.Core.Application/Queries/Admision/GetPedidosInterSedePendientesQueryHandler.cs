using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPedidosInterSedePendientesQueryHandler : IRequestHandler<GetPedidosInterSedePendientesQuery, List<PedidoInterSedeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPedidosInterSedePendientesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PedidoInterSedeDto>> Handle(GetPedidosInterSedePendientesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PedidosInterSede
                .Include(p => p.SedeSolicitante)
                .Include(p => p.SedeProveedora)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Insumo)
                .Where(p => p.Estado == EstadoPedidoInterSede.Solicitado || p.Estado == EstadoPedidoInterSede.Aprobado)
                .OrderByDescending(p => p.FechaCreacion);

            var items = await query.ToListAsync(cancellationToken);

            return items.Select(p => new PedidoInterSedeDto
            {
                Id = p.Id,
                Correlativo = p.Correlativo,
                SedeSolicitanteId = p.SedeSolicitanteId,
                SedeSolicitanteNombre = p.SedeSolicitante.Nombre,
                SedeProveedoraId = p.SedeProveedoraId,
                SedeProveedoraNombre = p.SedeProveedora.Nombre,
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
                    InsumoNombre = d.Insumo.Nombre,
                    InsumoCodigo = d.Insumo.Codigo,
                    CantidadSolicitada = d.CantidadSolicitada,
                    CantidadDespachada = d.CantidadDespachada,
                    CantidadRecibida = d.CantidadRecibida
                }).ToList()
            }).ToList();
        }
    }
}
