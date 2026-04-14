using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Common
{
    /// <summary>
    /// [PHASE-5] Base class for entities that support Domain Events.
    /// Standardizes aggregate root behaviors across the project.
    /// </summary>
    public abstract class BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
