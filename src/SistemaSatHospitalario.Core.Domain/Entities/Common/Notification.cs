using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Common
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Type { get; private set; } // Success, Info, Warning, Error

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario destino de la notificación.</summary>
        public Guid? TargetUserGuidId { get; private set; }
        public string? TargetRole { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? ActionUrl { get; private set; }

        protected Notification() { }

        public Notification(string title, string message, string type, Guid? targetUserGuidId = null, string? targetRole = null, string? actionUrl = null)
        {
            Id = Guid.NewGuid();
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Type = type ?? "Info";
            TargetUserGuidId = targetUserGuidId;
            TargetRole = targetRole;
            ActionUrl = actionUrl;
            Timestamp = DateTime.UtcNow;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
