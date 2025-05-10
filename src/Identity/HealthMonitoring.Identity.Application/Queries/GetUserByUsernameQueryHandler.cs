using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.DTOs;
using HealthMonitoring.Identity.Application.Queries;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Handlers.Queries
{
    /// <summary>
    /// Handler for GetUserByUsernameQuery
    /// </summary>
    public class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, Result<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByUsernameQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            // Get user by username
            var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);
            if (user == null)
            {
                return Result<UserDto>.Failure($"User with username '{request.Username}' not found");
            }

            // Map user to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                UserRoles = user.UserRoleMappings.Select(ur => new UserRoleDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name
                }).ToList()
            };

            return Result<UserDto>.Success(userDto);
        }
    }
}