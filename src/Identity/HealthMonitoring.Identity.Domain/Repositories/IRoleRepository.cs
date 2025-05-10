using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Models;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Identity.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Role entity
    /// </summary>
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Role> GetByRoleTypeAsync(HealthMonitoring.SharedKernel.DomainModels.Enums.UserRole roleType, CancellationToken cancellationToken = default);        Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        void Update(Role role);
        void Remove(Role role);
    }
}