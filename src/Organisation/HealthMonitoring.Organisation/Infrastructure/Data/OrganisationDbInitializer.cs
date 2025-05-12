using System;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Hospitals.Models;
using HealthMonitoring.Organisation.Features.Departments.Models;
using HealthMonitoring.Organisation.Features.Staff.Models;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Organisation.Infrastructure.Data
{
    /// <summary>
    /// Database initializer for seeding initial Organisation data
    /// </summary>
    public class OrganisationDbInitializer
    {
        private readonly OrganisationDbContext _context;
        private readonly ILogger<OrganisationDbInitializer> _logger;

        public OrganisationDbInitializer(
            OrganisationDbContext context,
            ILogger<OrganisationDbInitializer> logger)
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
            // Seed hospitals if they don't exist
            if (!await _context.Hospitals.AnyAsync())
            {
                var hospital = new Hospital(
                    "Central Hospital", 
                    "123 Main Street, City", 
                    "555-1234", 
                    "info@centralhospital.com");

                await _context.Hospitals.AddAsync(hospital);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeded hospital successfully.");

                // Seed departments for the hospital
                var departments = new[]
                {
                    new Department("Cardiology", "Heart and cardiovascular care", hospital.Id),
                    new Department("Neurology", "Brain and nervous system care", hospital.Id),
                    new Department("Pediatrics", "Children's healthcare", hospital.Id),
                    new Department("Oncology", "Cancer diagnosis and treatment", hospital.Id),
                    new Department("Emergency", "Emergency medical services", hospital.Id)
                };

                await _context.Departments.AddRangeAsync(departments);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeded departments successfully.");

                // Add a sample staff member for testing
                var department = departments[0]; // Cardiology department
                var staffMember = new StaffMember(
                    Guid.Parse("00000000-0000-0000-0000-000000000000"), // This would be a real user ID in a real environment
                    "John",
                    "Doe",
                    "Dr.",
                    department.Id,
                    UserRole.Doctor,
                    "MD12345"
                );

                await _context.StaffMembers.AddAsync(staffMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeded staff member successfully.");
            }
        }
    }
}