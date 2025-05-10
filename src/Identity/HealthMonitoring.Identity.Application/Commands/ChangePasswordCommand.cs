using System;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Commands
{
    /// <summary>
    /// Command for changing a user's password
    /// </summary>
    public class ChangePasswordCommand : ICommand<Result>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}