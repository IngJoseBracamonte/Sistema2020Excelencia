using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DocumentLog
    {
        public Guid Id { get; private set; }
        public string DocumentType { get; private set; } = string.Empty;
        public string ReferenceId { get; private set; } = string.Empty;
        public string Action { get; private set; } = string.Empty;

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioIdentityId { get; private set; }

        public DateTime Timestamp { get; private set; }
        public string? Details { get; private set; }

        protected DocumentLog() { }

        public DocumentLog(
            string documentType, 
            string referenceId, 
            string action, 
            Guid? usuarioIdentityId = null,
            string? userId = null, 
            string? userName = null, 
            string? details = null)
        {
            Id = Guid.NewGuid();
            DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
            ReferenceId = referenceId ?? throw new ArgumentNullException(nameof(referenceId));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioIdentityId = usuarioIdentityId ?? (Guid.TryParse(userId, out var parsed) ? parsed : (Guid?)null);
            Timestamp = DateTime.UtcNow;
            Details = details;
        }
    }
}