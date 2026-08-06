using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Services;

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

        public RegistrarDescarteCommandHandler(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task Handle(RegistrarDescarteCommand request, CancellationToken cancellationToken)
        {
            await _inventoryService.RecordDiscardAsync(
                request.InsumoId,
                request.Cantidad,
                request.Motivo,
                request.Usuario,
                request.SedeId,
                cancellationToken);
        }
    }
}
