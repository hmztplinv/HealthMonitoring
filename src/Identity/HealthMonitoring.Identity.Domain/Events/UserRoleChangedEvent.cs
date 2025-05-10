using System;
using HealthMonitoring.SharedKernel.EventModels;

namespace HealthMonitoring.Identity.Domain.Events
{
    /// <summary>
    /// Event raised when a user's role is changed
    /// </summary>
    public class UserRoleChangedEvent : BaseDomainEvent
    {
        public Guid UserId { get; }
        public Guid RoleId { get; }
        public string RoleName { get; }
        public bool IsAdded { get; }
        
        public UserRoleChangedEvent(Guid userId, Guid roleId, string roleName, bool isAdded)
        {
            UserId = userId;
            RoleId = roleId;
            RoleName = roleName;
            IsAdded = isAdded;
        }
    }
}