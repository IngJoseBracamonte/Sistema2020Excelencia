using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarDescarteCommand : IRequest
    {
        public Guid InsumoId { get; set; }
        public Guid? SedeId { get; set; }
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
    }

    public class RegistrarDescarteCommandHandler : IRequestHandler<RegistrarDescarteCommand>
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICurrentUserService _currentUserService;

        public RegistrarDescarteCommandHandler(IInventoryService inventoryService, ICurrentUserService currentUserService   )
        {
            _inventoryService = inventoryService;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RegistrarDescarteCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.RecordDiscardAsync(
                request.InsumoId,
                request.Cantidad,
                request.Motivo,
                _currentUserService.UserId,
                request.SedeId,
                cancellationToken);
        }
    }
}
