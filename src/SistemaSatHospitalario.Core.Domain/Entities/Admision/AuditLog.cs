using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// LEGACY (3FN): ID de usuario como texto. Fuente de verdad:
        /// <see cref="UsuarioIdentityId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioIdentityId. Columna legacy pendiente de DROP.")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que ejecutó la acción.</summary>
        public Guid? UsuarioIdentityId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
