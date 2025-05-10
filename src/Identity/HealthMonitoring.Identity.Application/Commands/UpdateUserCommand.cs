using System;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Commands
{
    /// <summary>
    /// Command for updating an existing user
    /// </summary>
    public class UpdateUserCommand : ICommand<Result>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
    }
}