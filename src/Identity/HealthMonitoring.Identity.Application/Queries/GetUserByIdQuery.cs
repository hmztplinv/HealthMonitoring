using System;
using HealthMonitoring.Identity.Application.DTOs;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Queries
{
    /// <summary>
    /// Query for getting a user by ID
    /// </summary>
    public class GetUserByIdQuery : IQuery<Result<UserDto>>
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}