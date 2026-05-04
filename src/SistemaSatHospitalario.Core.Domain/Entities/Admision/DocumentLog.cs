using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DocumentLog
    {
        public Guid Id { get; private set; }
        public string DocumentType { get; private set; }
        public string ReferenceId { get; private set; }
        public string Action { get; private set; }
        public string UserId { get; private set; }
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
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Timestamp = DateTime.UtcNow;
            Details = details;
        }
    }
}
