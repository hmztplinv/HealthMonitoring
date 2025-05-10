using System;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Identity.Application.DTOs
{
    /// <summary>
    /// DTO for Role entity
    /// </summary>
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserRole RoleType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}