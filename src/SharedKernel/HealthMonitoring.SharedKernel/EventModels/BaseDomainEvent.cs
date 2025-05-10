using System;

namespace HealthMonitoring.SharedKernel.EventModels
{
    /// <summary>
    /// Base class for all domain events
    /// </summary>
    public abstract class BaseDomainEvent
    {
        /// <summary>
        /// The unique identifier for the event
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// The timestamp when the event occurred
        /// </summary>
        public DateTime OccurredOn { get; }

        protected BaseDomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}