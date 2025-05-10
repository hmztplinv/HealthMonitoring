using System;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Commands
{
    /// <summary>
    /// Command for removing a role from a user
    /// </summary>
    public class RemoveRoleCommand : ICommand<Result>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}