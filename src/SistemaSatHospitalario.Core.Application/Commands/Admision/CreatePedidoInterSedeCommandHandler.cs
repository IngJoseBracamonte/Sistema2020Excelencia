using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreatePedidoInterSedeCommandHandler : IRequestHandler<CreatePedidoInterSedeCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreatePedidoInterSedeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreatePedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            // Regla de Negocio 1: Flujo Unidireccional - La Sede Proveedora SIEMPRE es la Sede Principal
            var sedeProveedoraId = SeedConstants.SedeId_Principal;

            // Regla Técnica 2: Correlativo Atómico (PED-YYYY-XXXX) mediante OrderByDescending
            var year = DateTime.UtcNow.Year.ToString();
            var latestCorrelativo = await _context.PedidosInterSede
                .Where(p => p.Correlativo.StartsWith($"PED-{year}-"))
                .OrderByDescending(p => p.Correlativo)
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

            var pedido = new PedidoInterSede(
                correlativo,
                request.Dto.SedeSolicitanteId,
                sedeProveedoraId,
                request.Usuario ?? "Sistema",
                request.Dto.Observaciones
            );

            foreach (var linea in request.Dto.Lineas)
            {
                var detalle = new PedidoInterSedeDetalle(linea.InsumoId, linea.CantidadSolicitada);
                pedido.AgregarDetalle(detalle);
            }

            _context.PedidosInterSede.Add(pedido);
            await _context.SaveChangesAsync(cancellationToken);

            return pedido.Id;
        }
    }
}
