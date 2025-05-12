using System;
using HealthMonitoring.SharedKernel.DomainModels;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.Organisation.Features.Departments.Models;

namespace HealthMonitoring.Organisation.Features.Staff.Models
{
    /// <summary>
    /// Represents a staff member in the healthcare organization
    /// </summary>
    public class StaffMember : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Title { get; private set; }
        public Guid DepartmentId { get; private set; }
        public UserRole StaffRole { get; private set; }
        public string LicenseNumber { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual Department Department { get; private set; }

        protected StaffMember() : base()
        {
        }

        public StaffMember(
            Guid userId,
            string firstName,
            string lastName,
            string title,
            Guid departmentId,
            UserRole staffRole,
            string licenseNumber) : this()
        {
            UserId = userId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Title = title ?? string.Empty;
            DepartmentId = departmentId;
            StaffRole = staffRole;
            LicenseNumber = licenseNumber ?? string.Empty;
            IsActive = true;
        }

        public void UpdateDetails(
            string firstName,
            string lastName,
            string title,
            UserRole staffRole,
            string licenseNumber)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Title = title ?? string.Empty;
            StaffRole = staffRole;
            LicenseNumber = licenseNumber ?? string.Empty;
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

        public void TransferToDepartment(Guid departmentId)
        {
            DepartmentId = departmentId;
            SetUpdatedAt();
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}