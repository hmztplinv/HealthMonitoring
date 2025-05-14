using System;
using HealthMonitoring.SharedKernel.EventModels;

namespace HealthMonitoring.Identity.Domain.Events
{
    /// <summary>
    /// Event raised when a new user is created
    /// </summary>
    public class UserCreatedEvent : BaseIntegrationEvent
    {
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Guid[] RoleIds { get; }
        
        public UserCreatedEvent(Guid userId, string username, string email, string firstName, string lastName, Guid[] roleIds)
        {
            UserId = userId;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            RoleIds = roleIds;
        }
    }
}