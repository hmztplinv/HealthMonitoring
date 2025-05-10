using System;

namespace HealthMonitoring.SharedKernel.EventModels
{
    /// <summary>
    /// Base class for all integration events
    /// </summary>
    public abstract class BaseIntegrationEvent
    {
        /// <summary>
        /// The unique identifier for the event
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// The timestamp when the event occurred
        /// </summary>
        public DateTime OccurredOn { get; }
        
        /// <summary>
        /// The type of the event
        /// </summary>
        public string EventType => GetType().Name;

        protected BaseIntegrationEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}