using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Repositories;

namespace HealthMonitoring.Identity.Domain.Services
{
    /// <summary>
    /// Unit of work interface for managing transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}