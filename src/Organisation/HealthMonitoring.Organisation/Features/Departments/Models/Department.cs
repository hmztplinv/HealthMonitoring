using System;
using System.Collections.Generic;
using HealthMonitoring.SharedKernel.DomainModels;
using HealthMonitoring.Organisation.Features.Hospitals.Models;
using HealthMonitoring.Organisation.Features.Staff.Models;

namespace HealthMonitoring.Organisation.Features.Departments.Models
{
    /// <summary>
    /// Represents a department within a hospital
    /// </summary>
    public class Department : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid HospitalId { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual Hospital Hospital { get; private set; }
        public virtual ICollection<StaffMember> StaffMembers { get; private set; }

        protected Department() : base()
        {
            StaffMembers = new List<StaffMember>();
        }

        public Department(
            string name,
            string description,
            Guid hospitalId) : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            HospitalId = hospitalId;
            IsActive = true;
        }

        public void UpdateDetails(
            string name,
            string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
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

        public void TransferToHospital(Guid hospitalId)
        {
            HospitalId = hospitalId;
            SetUpdatedAt();
        }
    }
}