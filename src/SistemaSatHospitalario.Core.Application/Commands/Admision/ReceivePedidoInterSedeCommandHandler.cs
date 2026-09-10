using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ReceivePedidoInterSedeCommandHandler : IRequestHandler<ReceivePedidoInterSedeCommand>
    {
        private readonly IInventoryService _inventoryService;

        private readonly ICurrentUserService _currentUserService;

        public ReceivePedidoInterSedeCommandHandler(IInventoryService inventoryService, ICurrentUserService currentUserService)
        {
            _inventoryService = inventoryService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(ReceivePedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.ReceivePedidoAsync(request.PedidoId, _currentUserService.UserId, request.Discrepancias, cancellationToken);
        }
    }
}
