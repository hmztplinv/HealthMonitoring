using System;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Commands
{
    /// <summary>
    /// Command to delete a user
    /// </summary>
    public class DeleteUserCommand : ICommand<Result>
    {
        /// <summary>
        /// The ID of the user to delete
        /// </summary>
        public Guid Id { get; set; }
    }
} 