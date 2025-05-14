// src/ApiGateway/HealthMonitoring.ApiGateway/Models/UserManagementDto.cs
namespace HealthMonitoring.ApiGateway.Models
{
    public class CreateStaffUserRequest
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid[] RoleIds { get; set; } = Array.Empty<Guid>();
        public int StaffRole { get; set; } 
        public Guid DepartmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
    }

    public class CreatePatientUserRequest
    {
        public Guid UserId { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid[] RoleIds { get; set; } = Array.Empty<Guid>();
        public string IdentificationNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
    }
}