using System;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Commands
{
    /// <summary>
    /// Command for creating a new user
    /// </summary>
    public class CreateUserCommand : ICommand<Result<Guid>>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid[] RoleIds { get; set; }
    }
}