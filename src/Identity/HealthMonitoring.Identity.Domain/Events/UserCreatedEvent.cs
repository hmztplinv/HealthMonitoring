using System;
using HealthMonitoring.SharedKernel.EventModels;

namespace HealthMonitoring.Identity.Domain.Events
{
    /// <summary>
    /// Event raised when a new user is created
    /// </summary>
    public class UserCreatedEvent : BaseDomainEvent
    {
        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        
        public UserCreatedEvent(Guid userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
        }
    }
}