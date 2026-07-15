using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CompleteMonitoringOrderCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
    }

    public class CompleteMonitoringOrderCommandHandler : IRequestHandler<CompleteMonitoringOrderCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notification;

        public CompleteMonitoringOrderCommandHandler(IApplicationDbContext context, INotificationService notification)
        {
            _context = context;
            _notification = notification;
        }

        public async Task<bool> Handle(CompleteMonitoringOrderCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            if (cuenta == null) return false;

            cuenta.ActualizarProcesamiento(EstadoConstants.ProcesamientoProcesada);
            await _context.SaveChangesAsync(cancellationToken);

            // Notificar via SignalR
            await _notification.SendNotificationToGroupAsync(
                "ProcessingOrders", 
                "Orden Procesada", 
                $"La orden #{cuenta.LegacyOrderId} ha sido procesada manualmente.",
                "Laboratory",
                new { CuentaId = cuenta.Id, LegacyOrderId = cuenta.LegacyOrderId, Status = "PROCESADA" },
                cancellationToken);

            return true;
        }
    }
}
