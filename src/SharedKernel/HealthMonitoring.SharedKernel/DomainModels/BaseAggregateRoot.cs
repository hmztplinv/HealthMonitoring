using System;
using System.Collections.Generic;
using HealthMonitoring.SharedKernel.EventModels;

namespace HealthMonitoring.SharedKernel.DomainModels
{
    /// <summary>
    /// Base class for aggregate roots in the domain
    /// </summary>
    public abstract class BaseAggregateRoot : BaseEntity
    {
        private readonly List<BaseDomainEvent> _domainEvents = new List<BaseDomainEvent>();
        
        /// <summary>
        /// Domain events raised by this aggregate
        /// </summary>
        public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseAggregateRoot() : base()
        {
        }

        protected BaseAggregateRoot(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Adds a domain event to this aggregate
        /// </summary>
        /// <param name="domainEvent">The domain event to add</param>
        protected void AddDomainEvent(BaseDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clears all domain events from this aggregate
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}