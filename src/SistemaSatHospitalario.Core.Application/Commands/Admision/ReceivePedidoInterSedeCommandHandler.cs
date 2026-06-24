using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ReceivePedidoInterSedeCommandHandler : IRequestHandler<ReceivePedidoInterSedeCommand>
    {
        private readonly IInventoryService _inventoryService;

        public ReceivePedidoInterSedeCommandHandler(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task Handle(ReceivePedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.ReceivePedidoAsync(request.PedidoId, request.Usuario, request.Discrepancias, cancellationToken);
        }
    }
}
