using System;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.SharedKernel.EventModels
{
    public class StaffCreatedEvent : BaseIntegrationEvent
    {
        public Guid StaffId { get; }
        public Guid UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Guid DepartmentId { get; }
        public UserRole StaffRole { get; }

        public StaffCreatedEvent(
            Guid staffId,
            Guid userId,
            string firstName,
            string lastName,
            Guid departmentId,
            UserRole staffRole)
        {
            StaffId = staffId;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            DepartmentId = departmentId;
            StaffRole = staffRole;
        }
    }
}
