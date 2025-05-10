using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Helpers;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Handlers.Commands
{
    /// <summary>
    /// Handler for ChangePasswordCommand
    /// </summary>
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Failure($"User with ID {request.UserId} not found");
            }

            // Verify current password
            if (!CryptographyHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return Result.Failure("Current password is incorrect");
            }

            // Set new password
            var (passwordHash, passwordSalt) = CryptographyHelper.HashPassword(request.NewPassword);
            user.SetPassword(passwordHash, passwordSalt);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}