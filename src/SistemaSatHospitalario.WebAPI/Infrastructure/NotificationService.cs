using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.WebAPI.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> _hubContext)
        {
            this._hubContext = _hubContext;
        }

        public async Task SendValidationAlertAsync(string title, string message, string category, object? metadata = null, CancellationToken ct = default)
        {
            await _hubContext.Clients.Group(NotificationHub.AuditGroup).SendAsync("ReceiveNotification", new NotificationDto
            {
                Title = title,
                Message = message,
                Category = category,
                Severity = "Info",
                Timestamp = DateTime.UtcNow,
                Metadata = metadata
            }, ct);
        }
    }
}
