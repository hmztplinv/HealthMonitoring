using System;
using System.Collections.Generic;

namespace HealthMonitoring.Identity.Application.DTOs
{
    /// <summary>
    /// DTO for User entity
    /// </summary>
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<UserRoleDto> UserRoles { get; set; }
    }
}