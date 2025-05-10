using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Models;
using HealthMonitoring.Identity.Domain.Repositories;
using HealthMonitoring.Identity.Infrastructure.Data;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Identity.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of IRoleRepository
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _context;

        public RoleRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        }

        public async Task<Role> GetByRoleTypeAsync(UserRole roleType, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleType == roleType, cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await _context.Roles.AddAsync(role, cancellationToken);
        }

        public void Update(Role role)
        {
            _context.Entry(role).State = EntityState.Modified;
        }

        public void Remove(Role role)
        {
            _context.Roles.Remove(role);
        }
    }
}