using System;

namespace HealthMonitoring.SharedKernel.EventModels
{
    public class PatientCreatedEvent : BaseIntegrationEvent
    {
        public Guid PatientId { get; }
        public Guid UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        
        public PatientCreatedEvent(
            Guid patientId,
            Guid userId,
            string firstName,
            string lastName)
        {
            PatientId = patientId;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}