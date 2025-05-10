using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Repositories;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.Identity.Infrastructure.Data;
using HealthMonitoring.Identity.Infrastructure.Repositories;

namespace HealthMonitoring.Identity.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IUnitOfWork
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _context;
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;

        public UnitOfWork(IdentityDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}