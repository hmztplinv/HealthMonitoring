using System.Collections.Generic;
using HealthMonitoring.Identity.Application.DTOs;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Queries
{
    /// <summary>
    /// Query for getting all roles
    /// </summary>
    public class GetAllRolesQuery : IQuery<Result<List<RoleDto>>>
    {
    }
}