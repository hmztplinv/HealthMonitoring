using System;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Commands
{
    /// <summary>
    /// Command for assigning a role to a user
    /// </summary>
    public class AssignRoleCommand : ICommand<Result>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}