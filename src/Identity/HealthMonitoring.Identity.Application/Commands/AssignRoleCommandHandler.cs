using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Handlers.Commands
{
    /// <summary>
    /// Handler for AssignRoleCommand
    /// </summary>
    public class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
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

            // Add role to user
            user.AddRole(role);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}