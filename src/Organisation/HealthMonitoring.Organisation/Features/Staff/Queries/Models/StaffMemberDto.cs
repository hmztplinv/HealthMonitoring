// StaffMemberDto.cs
using System;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Organisation.Features.Staff.Queries.Models
{
    public class StaffMemberDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid HospitalId { get; set; }
        public string HospitalName { get; set; }
        public UserRole StaffRole { get; set; }
        public string RoleName { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}