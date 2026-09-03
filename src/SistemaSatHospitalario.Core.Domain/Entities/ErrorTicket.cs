using System;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class ErrorTicket
    {
        public Guid Id { get; set; }
        public string RequestPath { get; set; } = string.Empty;
        public string MetodoHTTP { get; set; } = string.Empty;
        public string MensajeExcepcion { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioAsociadoId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioAsociadoId. Columna legacy pendiente de DROP.")]
        public string? UsuarioAsociado { get; set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario asociado al error.</summary>
        public Guid? UsuarioAsociadoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Resuelto { get; set; }
        public string? ComentariosResolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="ResueltoPorId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar ResueltoPorId. Columna legacy pendiente de DROP.")]
        public string? ResueltoPor { get; set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que resolvió el ticket.</summary>
        public Guid? ResueltoPorId { get; set; }
    }
}
