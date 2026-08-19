using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
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
            // Auto-sincronización de solicitudes quirúrgicas pendientes huérfanas
            var solicitudesQuirofano = await _context.SolicitudesInsumosCirugia
                .Include(s => s.OrdenCirugia)
                    .ThenInclude(o => o.Paciente)
                .Where(s => s.EstadoSolicitud == EstadoSolicitudInsumoConstants.Pendiente)
                .ToListAsync(cancellationToken);

            bool hayCambios = false;
            foreach (var sol in solicitudesQuirofano)
            {
                var tag = $"[CIRUGIA_ADHOC:{sol.Id}:";
                bool yaExiste = await _context.PedidosInterSede
                    .AnyAsync(p => p.Observaciones != null && p.Observaciones.Contains(tag), cancellationToken);

                if (!yaExiste)
                {
                    var year = DateTime.UtcNow.Year.ToString();
                    var latestCorrelativo = await _context.PedidosInterSede
                        .Where(p => p.Correlativo.StartsWith($"PED-{year}-"))
                        .OrderByDescending(p => p.Correlativo.Length)
                        .ThenByDescending(p => p.Correlativo)
                        .Select(p => p.Correlativo)
                        .FirstOrDefaultAsync(cancellationToken);

                    int nextSeq = 1;
                    if (!string.IsNullOrEmpty(latestCorrelativo) && latestCorrelativo.Length >= 13)
                    {
                        var numPart = latestCorrelativo.Substring(latestCorrelativo.LastIndexOf('-') + 1);
                        if (int.TryParse(numPart, out int lastSeq))
                        {
                            nextSeq = lastSeq + 1;
                        }
                    }
                    var correlativo = $"PED-{year}-{nextSeq.ToString().PadLeft(4, '0')}";
                    var pacNombre = sol.OrdenCirugia?.Paciente != null 
                        ? sol.OrdenCirugia.Paciente.NombreCorto
                        : "Paciente Quirúrgico";

                    var pedido = new PedidoInterSede(
                        correlativo,
                        SeedConstants.SedeId_Cirugia,
                        sol.AlmacenOrigenId != Guid.Empty ? sol.AlmacenOrigenId : SeedConstants.SedeId_Principal,
                        sol.UsuarioSolicitud ?? "Sistema",
                        $"[CIRUGIA_ADHOC:{sol.Id}:{sol.OrdenCirugiaId}] Paciente: {pacNombre} | Urgencia: {sol.Observaciones}"
                    );
                    pedido.AgregarDetalle(new PedidoInterSedeDetalle(sol.InsumoId, sol.CantidadSolicitada));
                    _context.PedidosInterSede.Add(pedido);
                    hayCambios = true;
                }
            }

            if (hayCambios)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            var query = _context.PedidosInterSede
                .IgnoreQueryFilters()
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
