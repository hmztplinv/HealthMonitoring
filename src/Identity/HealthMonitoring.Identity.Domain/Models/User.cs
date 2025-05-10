using System;
using System.Collections.Generic;
using HealthMonitoring.SharedKernel.DomainModels;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Identity.Domain.Models
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    public class User : BaseAggregateRoot
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string PasswordSalt { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginDate { get; private set; }

        // Navigation properties
        public virtual ICollection<UserRoleMapping> UserRoleMappings { get; private set; }

        protected User() : base()
        {
            UserRoleMappings = new List<UserRoleMapping>();
        }

        public User(
            string userName,
            string email,
            string firstName,
            string lastName) : this()
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            IsActive = true;
        }

        public void SetPassword(string passwordHash, string passwordSalt)
        {
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            PasswordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
            SetUpdatedAt();
        }

        public void UpdatePersonalInfo(string firstName, string lastName, string email)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            SetUpdatedAt();
        }

        public void SetLastLoginDate(DateTime loginDate)
        {
            LastLoginDate = loginDate;
            SetUpdatedAt();
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }

        public void AddRole(Role role)
        {
            var userRole = new UserRoleMapping(this, role);
            UserRoleMappings.Add(userRole);
            SetUpdatedAt();
        }

        public void RemoveRole(Role role)
        {
            var userRole = FindUserRoleMapping(role.Id);
            if (userRole != null)
            {
                UserRoleMappings.Remove(userRole);
                SetUpdatedAt();
            }
        }

        private UserRoleMapping FindUserRoleMapping(Guid roleId)
        {
            foreach (var userRole in UserRoleMappings)
            {
                if (userRole.RoleId == roleId)
                {
                    return userRole;
                }
            }
            return null;
        }
    }
}