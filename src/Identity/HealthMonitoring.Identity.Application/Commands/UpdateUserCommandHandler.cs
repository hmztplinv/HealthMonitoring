using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.ErrorHandling;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Handlers.Commands
{
    /// <summary>
    /// Handler for UpdateUserCommand
    /// </summary>
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return Result.Failure($"User with ID {request.Id} not found");
            }

            // Check if email is already used by another user
            var existingUserWithEmail = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != request.Id)
            {
                return Result.Failure($"Email '{request.Email}' is already used by another user");
            }

            // Update user
            user.UpdatePersonalInfo(request.FirstName, request.LastName, request.Email);

            // Update active status
            if (user.IsActive != request.IsActive)
            {
                if (request.IsActive)
                {
                    user.Activate();
                }
                else
                {
                    user.Deactivate();
                }
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}