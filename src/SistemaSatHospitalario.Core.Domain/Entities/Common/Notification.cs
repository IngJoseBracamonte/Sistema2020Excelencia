using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Common
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Type { get; private set; } // Success, Info, Warning, Error

        /// <summary>
        /// LEGACY (3FN): ID de usuario destino como texto. Fuente de verdad:
        /// <see cref="TargetUserGuidId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar TargetUserGuidId. Columna legacy pendiente de DROP.")]
        public string? TargetUserId { get; private set; } // Null if for everyone or specific roles

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario destino de la notificación.</summary>
        public Guid? TargetUserGuidId { get; private set; }
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
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            TargetUserId = targetUserId;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            TargetUserGuidId = Guid.TryParse(targetUserId, out var parsed) ? parsed : (Guid?)null;
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
