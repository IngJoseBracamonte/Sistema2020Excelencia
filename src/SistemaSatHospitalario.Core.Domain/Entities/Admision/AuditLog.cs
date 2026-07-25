using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
