using System;

namespace HealthMonitoring.Identity.Application.DTOs
{
    /// <summary>
    /// DTO for UserRole entity
    /// </summary>
    public class UserRoleDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}