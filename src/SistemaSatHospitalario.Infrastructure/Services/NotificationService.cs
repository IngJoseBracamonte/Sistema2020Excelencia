using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Common;
using SistemaSatHospitalario.Infrastructure.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IApplicationDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, IApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task SendValidationAlertAsync(string title, string message, string category, object? metadata = null, CancellationToken ct = default)
        {
            var dto = new NotificationDto
            {
                Title = title,
                Message = message,
                Category = category,
                Severity = "Warning",
                Metadata = metadata
            };

            await _hubContext.Clients.Group(NotificationHub.AuditGroup).SendAsync("ReceiveNotification", dto, ct);
        }

        public async Task SendNotificationToGroupAsync(string groupName, string title, string message, string category = "General", object? metadata = null, CancellationToken ct = default)
        {
            var dto = new NotificationDto
            {
                Title = title,
                Message = message,
                Category = category,
                Severity = "Info",
                Metadata = metadata
            };

            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", dto, ct);
        }

        public async Task CreatePersistentNotificationAsync(string title, string message, string type, string? targetUserId = null, string? targetRole = null, string? actionUrl = null, CancellationToken ct = default)
        {
            // 1. Persist to Database
            var notification = new Notification(title, message, type, targetUserId, targetRole, actionUrl);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(ct);

            // 2. Notify in Real-time via SignalR
            var dto = new NotificationDto
            {
                Title = title,
                Message = message,
                Category = "Silent",
                Severity = type,
                Metadata = new { notification.Id, actionUrl }
            };

            if (!string.IsNullOrEmpty(targetUserId))
            {
                await _hubContext.Clients.User(targetUserId).SendAsync("ReceiveSilentNotification", dto, ct);
            }
            else if (!string.IsNullOrEmpty(targetRole))
            {
                await _hubContext.Clients.Group(targetRole).SendAsync("ReceiveSilentNotification", dto, ct);
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveSilentNotification", dto, ct);
            }
        }
    }
}
