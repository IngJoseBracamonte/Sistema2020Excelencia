using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Common
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Type { get; private set; } // Success, Info, Warning, Error
        public string? TargetUserId { get; private set; } // Null if for everyone or specific roles
        public string? TargetRole { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? ActionUrl { get; private set; }

        protected Notification() { }

        public Notification(string title, string message, string type, string? targetUserId = null, string? targetRole = null, string? actionUrl = null)
        {
            Id = Guid.NewGuid();
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Type = type ?? "Info";
            TargetUserId = targetUserId;
            TargetRole = targetRole;
            IsRead = false;
            Timestamp = DateTime.UtcNow;
            ActionUrl = actionUrl;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
