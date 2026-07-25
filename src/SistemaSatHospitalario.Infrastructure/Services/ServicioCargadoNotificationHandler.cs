using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Notifications;
using SistemaSatHospitalario.Infrastructure.Hubs;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    public class ServicioCargadoNotificationHandler : INotificationHandler<ServicioCargadoNotification>
    {
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly ILogger<ServicioCargadoNotificationHandler> _logger;

        public ServicioCargadoNotificationHandler(
            IHubContext<DashboardHub> hubContext,
            ILogger<ServicioCargadoNotificationHandler> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task Handle(ServicioCargadoNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[SIGNALR-DISPATCH] Notificando despacho de servicio {TipoServicio} para paciente {PacienteId} ({NombrePaciente})",
                notification.TipoServicio, notification.PacienteId, notification.NombrePaciente);

            var payload = new
            {
                tipoServicio = notification.TipoServicio,
                origenCarga = notification.OrigenCarga,
                pacienteId = notification.PacienteId,
                nombrePaciente = notification.NombrePaciente,
                areaOrigen = notification.AreaOrigen,
                servicioDescripcion = notification.ServicioDescripcion,
                medicoId = notification.MedicoId,
                horaCita = notification.HoraCita?.ToString("o"),
                fechaCarga = notification.FechaCarga.ToString("o")
            };

            string groupName = notification.TipoServicio.ToUpperInvariant() switch
            {
                "RX" => "AsistenteRX",
                "TOMO" => "AsistenteTomografia",
                "LAB" or "LABORATORIO" => "AsistenteLaboratorio",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(groupName))
            {
                // Notificación a grupo de asistentes de especialidad
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveDispatchEvent", payload, cancellationToken);
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveTicketUpdate", payload, cancellationToken);
            }

            if (notification.MedicoId.HasValue && notification.MedicoId.Value != Guid.Empty)
            {
                // Notificación a la agenda del médico tratante
                string medicoGroup = $"Medico_{notification.MedicoId.Value}";
                await _hubContext.Clients.Group(medicoGroup).SendAsync("ReceiveDispatchEvent", payload, cancellationToken);
                await _hubContext.Clients.Group(medicoGroup).SendAsync("ReceiveTicketUpdate", payload, cancellationToken);
            }

            // También emitir broadcast general en ReceiveTicketUpdate para compatibilidad con dashboards de administración
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", payload, cancellationToken);
        }
    }
}
