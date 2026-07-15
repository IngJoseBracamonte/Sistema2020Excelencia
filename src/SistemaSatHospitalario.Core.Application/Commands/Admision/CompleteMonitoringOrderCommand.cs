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
        public string UsuarioCompleta { get; set; } = "Bioanalista";
    }

    public class CompleteMonitoringOrderCommandHandler : IRequestHandler<CompleteMonitoringOrderCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notification;
        private readonly IUserAuditLogger _auditLogger;

        public CompleteMonitoringOrderCommandHandler(IApplicationDbContext context, INotificationService notification, IUserAuditLogger auditLogger)
        {
            _context = context;
            _notification = notification;
            _auditLogger = auditLogger;
        }

        public async Task<bool> Handle(CompleteMonitoringOrderCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            if (cuenta == null) return false;

            cuenta.ActualizarProcesamiento(EstadoConstants.ProcesamientoProcesada);
            await _context.SaveChangesAsync(cancellationToken);

            // Registrar en el Log de Auditoria del Usuario
            await _auditLogger.LogActionAsync(
                request.UsuarioCompleta,
                "PROCESAMIENTO_LABORATORIO",
                cuenta.Id,
                $"Orden de Laboratorio #{cuenta.LegacyOrderId} marcada manualmente como PROCESADA."
            );

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
