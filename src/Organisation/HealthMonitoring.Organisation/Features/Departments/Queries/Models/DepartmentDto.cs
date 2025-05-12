// DepartmentDto.cs
using System;

namespace HealthMonitoring.Organisation.Features.Departments.Queries.Models
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid HospitalId { get; set; }
        public string HospitalName { get; set; }
        public bool IsActive { get; set; }
        public int StaffCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}