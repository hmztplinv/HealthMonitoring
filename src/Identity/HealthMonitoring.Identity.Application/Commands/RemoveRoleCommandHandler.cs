using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Handlers.Commands
{
    /// <summary>
    /// Handler for RemoveRoleCommand
    /// </summary>
    public class RemoveRoleCommandHandler : ICommandHandler<RemoveRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        {
            // Get user by ID
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.Failure($"User with ID {request.UserId} not found");
            }

            // Get role by ID
            var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                return Result.Failure($"Role with ID {request.RoleId} not found");
            }

            // Remove role from user
            user.RemoveRole(role);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}