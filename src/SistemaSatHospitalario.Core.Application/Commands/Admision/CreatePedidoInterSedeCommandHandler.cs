using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
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
            // Generar correlativo PED-YYYY-XXXX
            var year = DateTime.UtcNow.Year.ToString();
            var count = await _context.PedidosInterSede
                .CountAsync(p => p.Correlativo.StartsWith($"PED-{year}-"), cancellationToken);
            
            var correlativo = $"PED-{year}-{(count + 1).ToString().PadLeft(4, '0')}";

            var pedido = new PedidoInterSede(
                correlativo,
                request.Dto.SedeSolicitanteId,
                request.Dto.SedeProveedoraId,
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
