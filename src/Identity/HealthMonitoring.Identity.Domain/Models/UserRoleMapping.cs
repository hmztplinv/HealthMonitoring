using System;
using HealthMonitoring.SharedKernel.DomainModels;

namespace HealthMonitoring.Identity.Domain.Models
{
    /// <summary>
    /// Represents a many-to-many relationship between users and roles
    /// </summary>
    public class UserRoleMapping : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; }
        public virtual Role Role { get; private set; }

        protected UserRoleMapping() : base()
        {
        }

        public UserRoleMapping(User user, Role role) : this()
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            
            Role = role ?? throw new ArgumentNullException(nameof(role));
            RoleId = role.Id;
        }
    }
}