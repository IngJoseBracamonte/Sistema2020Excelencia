using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly ICurrentUserService _currentUserService;

        public RechazarPedidoInterSedeCommandHandler(IInventoryService inventoryService, ICurrentUserService currentUserService)
        {
            _inventoryService = inventoryService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RechazarPedidoInterSedeCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.RejectPedidoAsync(request.PedidoId, _currentUserService.UserId, request.Motivo, cancellationToken);
        }
    }
}
