using System;

namespace SistemaSatHospitalario.Core.Domain.Common
{
    /// <summary>
    /// [PHASE-5] Base class for Domain Events.
    /// Enables decoupling of secondary processes from the core business logic.
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; }
    }
}
