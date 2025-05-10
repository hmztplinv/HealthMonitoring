using System.Collections.Generic;
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
    /// Handler for GetAllUsersQuery
    /// </summary>
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<List<UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            // Get all users
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            // Map users to DTOs
            var userDtos = users.Select(user => new UserDto
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
            }).ToList();

            return Result<List<UserDto>>.Success(userDtos);
        }
    }
}