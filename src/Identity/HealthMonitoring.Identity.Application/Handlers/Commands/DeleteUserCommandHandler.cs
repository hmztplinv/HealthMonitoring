using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Results;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Identity.Application.Handlers.Commands
{
    /// <summary>
    /// Handler for DeleteUserCommand
    /// </summary>
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            
            if (user == null)
            {
                _logger.LogWarning("Attempt to delete non-existent user with ID: {UserId}", request.Id);
                return Result.Failure($"User with ID {request.Id} not found");
            }

            try
            {
                // Delete user
                _unitOfWork.Users.Remove(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("User deleted successfully: {UserId}", user.Id);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user: {UserId}", request.Id);
                return Result.Failure($"Failed to delete user: {ex.Message}");
            }
        }
    }
} 