using System.Collections.Generic;
using HealthMonitoring.Identity.Application.DTOs;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Queries
{
    /// <summary>
    /// Query for getting all users
    /// </summary>
    public class GetAllUsersQuery : IQuery<Result<List<UserDto>>>
    {
    }
}