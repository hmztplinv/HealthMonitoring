using System;
using HealthMonitoring.SharedKernel.DomainModels;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Identity.Domain.Models
{
    /// <summary>
    /// Represents a role in the system
    /// </summary>
    public class Role : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public HealthMonitoring.SharedKernel.DomainModels.Enums.UserRole RoleType { get; private set; }

        protected Role() : base()
        {
        }

        public Role(string name, string description, HealthMonitoring.SharedKernel.DomainModels.Enums.UserRole roleType) : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            RoleType = roleType;
        }

        public void Update(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            SetUpdatedAt();
        }
    }
}