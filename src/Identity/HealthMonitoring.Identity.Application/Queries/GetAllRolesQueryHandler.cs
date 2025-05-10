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
    /// Handler for GetAllRolesQuery
    /// </summary>
    public class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, Result<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            // Get all roles
            var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);

            // Map roles to DTOs
            var roleDtos = roles.Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                RoleType = role.RoleType,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            }).ToList();

            return Result<List<RoleDto>>.Success(roleDtos);
        }
    }
}