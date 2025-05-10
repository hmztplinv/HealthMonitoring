using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Models;
using HealthMonitoring.Identity.Domain.Repositories;
using HealthMonitoring.Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Identity.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of IUserRepository
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;

        public UserRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoleMappings)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoleMappings)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoleMappings)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.UserRoleMappings)
                .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == username, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }
    }
}