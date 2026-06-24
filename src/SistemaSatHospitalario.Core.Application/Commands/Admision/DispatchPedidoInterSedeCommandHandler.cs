using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DispatchPedidoInterSedeCommandHandler : IRequestHandler<DispatchPedidoInterSedeCommand>
    {
        private readonly IInventoryService _inventoryService;

        public DispatchPedidoInterSedeCommandHandler(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task Handle(DispatchPedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.DispatchPedidoAsync(request.PedidoId, request.Usuario, cancellationToken);
        }
    }
}
