using System;

namespace HealthMonitoring.Organisation.Features.Hospitals.Queries.Models
{
    public class HospitalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentCount { get; set; }
        public int StaffCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}