using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Hubs
{
    /// <summary>
    /// Senior Notification Hub (V15.0).
    /// Centralizes real-time communication for technical validation and billing alerts.
    /// </summary>
    public class NotificationHub : Hub
    {
        public const string AuditGroup = "AuditGroup";
        public const string TechnicianGroup = "TechnicianGroup";

        public override async Task OnConnectedAsync()
        {
            // [PHASE-6] Automatic Group Assignment based on claims would happen here.
            // For now, we allow dynamic joining.
            await base.OnConnectedAsync();
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Sends a technical validation alert to specific administrative roles.
        /// </summary>
        public async Task SendValidationAlert(NotificationDto notification)
        {
            await Clients.Group(AuditGroup).SendAsync("ReceiveNotification", notification);
        }
    }

    public class NotificationDto
    {
        public string Title { get; set; } = "Alerta de Sistema";
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = "General"; // Cita, RX, Tomografia, Factura
        public string Severity { get; set; } = "Info"; // Info, Warning, Critical
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object? Metadata { get; set; }
    }
}
