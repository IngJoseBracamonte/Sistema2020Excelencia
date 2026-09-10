using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DispatchPedidoInterSedeCommandHandler : IRequestHandler<DispatchPedidoInterSedeCommand>
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICurrentUserService _currentUserService;

        public DispatchPedidoInterSedeCommandHandler(IInventoryService inventoryService, ICurrentUserService currentUserService)
        {
            _inventoryService = inventoryService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DispatchPedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.DispatchPedidoAsync(
                request.PedidoId, 
                _currentUserService.UserId, 
                request.CantidadesAprobadas, 
                request.ObservacionesPorDetalle, 
                cancellationToken);
        }
    }
}
