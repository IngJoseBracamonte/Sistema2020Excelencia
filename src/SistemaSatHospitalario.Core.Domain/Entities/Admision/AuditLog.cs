using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioIdentityId { get; set; }

        public string ActionType { get; set; } = string.Empty; // INSERT, UPDATE, DELETE, LOGIN
        public string? OldValue { get; set; } // JSON o snapshot del estado anterior
        public string? NewValue { get; set; } // JSON o snapshot del nuevo estado
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public AuditLog() { }

        public AuditLog(
            string actionType, 
            Guid? usuarioIdentityId = null, 
            string? userId = null, 
            string? oldValue = null, 
            string? newValue = null, 
            string? ipAddress = null)
        {
            Id = Guid.NewGuid();
            ActionType = actionType ?? throw new ArgumentNullException(nameof(actionType));
            UsuarioIdentityId = usuarioIdentityId ?? (Guid.TryParse(userId, out var parsed) ? parsed : (Guid?)null);
            OldValue = oldValue;
            NewValue = newValue;
            IpAddress = ipAddress;
            Timestamp = DateTime.UtcNow;
        }
    }
}