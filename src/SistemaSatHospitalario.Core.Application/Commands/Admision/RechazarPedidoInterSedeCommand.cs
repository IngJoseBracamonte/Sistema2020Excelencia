using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RechazarPedidoInterSedeCommand : IRequest
    {
        public Guid PedidoId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
    }

    public class RechazarPedidoInterSedeCommandHandler : IRequestHandler<RechazarPedidoInterSedeCommand>
    {
        private readonly IInventoryService _inventoryService;

        public RechazarPedidoInterSedeCommandHandler(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task Handle(RechazarPedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.RejectPedidoAsync(request.PedidoId, request.Usuario, request.Motivo, cancellationToken);
        }
    }
}
