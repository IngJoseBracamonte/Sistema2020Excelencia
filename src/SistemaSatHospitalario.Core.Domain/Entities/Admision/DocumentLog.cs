using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DocumentLog
    {
        public Guid Id { get; private set; }
        public string DocumentType { get; private set; }
        public string ReferenceId { get; private set; }
        public string Action { get; private set; }

        /// <summary>
        /// LEGACY (3FN): ID de usuario como texto. Fuente de verdad:
        /// <see cref="UsuarioIdentityId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioIdentityId. Columna legacy pendiente de DROP.")]
        public string UserId { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que ejecutó la acción.</summary>
        public Guid? UsuarioIdentityId { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre desnormalizado del usuario. Se deriva de
        /// <see cref="UsuarioIdentityId"/> vía Identity. Alias hasta el DROP.
        /// </summary>
        [Obsolete("Derivar de UsuarioIdentityId vía Identity. Columna legacy pendiente de DROP.")]
        public string UserName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? Details { get; private set; }

        protected DocumentLog() { }

        public DocumentLog(string documentType, string referenceId, string action, string userId, string userName, string? details = null)
        {
            Id = Guid.NewGuid();
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            ReferenceId = referenceId ?? throw new ArgumentNullException(nameof(referenceId));
            Action = action ?? throw new ArgumentNullException(nameof(action));
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioIdentityId = Guid.TryParse(userId, out var parsed) ? parsed : (Guid?)null;
            Timestamp = DateTime.UtcNow;
            Details = details;
        }
    }
}
