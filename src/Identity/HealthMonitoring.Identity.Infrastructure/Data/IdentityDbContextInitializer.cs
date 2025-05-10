using System;
using System.Linq;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Models;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Identity.Infrastructure.Data
{
    /// <summary>
    /// Database initializer for seeding initial data
    /// </summary>
    public class IdentityDbContextInitializer
    {
        private readonly IdentityDbContext _context;
        private readonly ILogger<IdentityDbContextInitializer> _logger;

        public IdentityDbContextInitializer(
            IdentityDbContext context,
            ILogger<IdentityDbContextInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.IsNpgsql())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // Seed roles if they don't exist
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Role("Administrator", "System administrator", UserRole.Administrator),
                    new Role("Doctor", "Healthcare provider", UserRole.Doctor),
                    new Role("Nurse", "Healthcare assistant", UserRole.Nurse),
                    new Role("Assistant", "Clinic assistant", UserRole.Assistant),
                    new Role("Patient", "Patient user", UserRole.Patient)
                };

                await _context.Roles.AddRangeAsync(roles);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeded roles successfully.");
            }

            // Seed admin user if it doesn't exist
            if (!await _context.Users.AnyAsync(u => u.UserName == "admin"))
            {
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleType == UserRole.Administrator);
                if (adminRole != null)
                {
                    // Create admin user
                    var adminUser = new User("admin", "admin@healthmonitoring.com", "Admin", "User");
                    
                    // Set password (using "Admin123!" as default password)
                    var (passwordHash, passwordSalt) = CryptographyHelper.HashPassword("Admin123!");
                    adminUser.SetPassword(passwordHash, passwordSalt);
                    
                    // Add admin role
                    adminUser.AddRole(adminRole);
                    
                    await _context.Users.AddAsync(adminUser);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Seeded admin user successfully.");
                }
            }
        }
    }
}