using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.System
{
    public class ErrorTicketCreatedEventHandler : INotificationHandler<ErrorTicketCreatedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ErrorTicketCreatedEventHandler> _logger;

        public ErrorTicketCreatedEventHandler(IEmailService emailService, ILogger<ErrorTicketCreatedEventHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(ErrorTicketCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var subject = $"🚨 CRITICAL ERROR: {notification.MensajeExcepcion} en {notification.RequestPath}";
                var htmlBody = $@"
                    <h2 style='color: #e53e3e;'>Alerta de Sistema: El SistemaSatHospitalario detectó un fallo</h2>
                    <p><strong>Ticket ID:</strong> {notification.TicketId}</p>
                    <p><strong>Ruta Afectada:</strong> {notification.RequestPath}</p>
                    <p><strong>Fecha:</strong> {notification.FechaCreacion:G} UTC</p>
                    <p><strong>Mensaje:</strong> {notification.MensajeExcepcion}</p>
                    <hr />
                    <h3>Revisión del StackTrace:</h3>
                    <pre style='background: #f4f4f4; padding: 10px; border-radius: 5px; font-size: 11px;'>{notification.StackTrace}</pre>
                ";

                await _emailService.SendEmailAsync("admin@hospital.com", subject, htmlBody);
                _logger.LogInformation("Administrator alerted via email for Ticket {TicketId}", notification.TicketId);
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError(ex, "Failed to send email alert for Ticket {TicketId}. The ticket was saved but notification failed.", notification.TicketId);
            }
        }
    }
}
