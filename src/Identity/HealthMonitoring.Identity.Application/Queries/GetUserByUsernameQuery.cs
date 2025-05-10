using HealthMonitoring.Identity.Application.DTOs;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Queries
{
    /// <summary>
    /// Query for getting a user by username
    /// </summary>
    public class GetUserByUsernameQuery : IQuery<Result<UserDto>>
    {
        public string Username { get; set; }

        public GetUserByUsernameQuery(string username)
        {
            Username = username;
        }
    }
}