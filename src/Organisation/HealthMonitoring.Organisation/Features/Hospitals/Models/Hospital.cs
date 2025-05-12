using System;
using System.Collections.Generic;
using HealthMonitoring.SharedKernel.DomainModels;
using HealthMonitoring.Organisation.Features.Departments.Models;

namespace HealthMonitoring.Organisation.Features.Hospitals.Models
{
    /// <summary>
    /// Represents a hospital in the system
    /// </summary>
    public class Hospital : BaseEntity
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<Department> Departments { get; private set; }

        protected Hospital() : base()
        {
            Departments = new List<Department>();
        }

        public Hospital(
            string name,
            string address,
            string phoneNumber,
            string email) : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PhoneNumber = phoneNumber ?? string.Empty;
            Email = email ?? string.Empty;
            IsActive = true;
        }

        public void UpdateDetails(
            string name,
            string address,
            string phoneNumber,
            string email)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PhoneNumber = phoneNumber ?? string.Empty;
            Email = email ?? string.Empty;
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
    }
}