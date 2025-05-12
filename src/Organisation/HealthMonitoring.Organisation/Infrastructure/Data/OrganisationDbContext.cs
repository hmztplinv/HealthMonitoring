using HealthMonitoring.Organisation.Features.Hospitals.Models;
using HealthMonitoring.Organisation.Features.Departments.Models;
using HealthMonitoring.Organisation.Features.Staff.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Infrastructure.Data
{
    /// <summary>
    /// EF Core DbContext for Organisation microservice
    /// </summary>
    public class OrganisationDbContext : DbContext
    {
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }

        public OrganisationDbContext(DbContextOptions<OrganisationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hospital configuration
            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.ToTable("Hospitals");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // Unique constraint on Name
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.HospitalId).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // Relationships
                entity.HasOne(e => e.Hospital)
                    .WithMany(h => h.Departments)
                    .HasForeignKey(e => e.HospitalId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint on Name within a Hospital
                entity.HasIndex(e => new { e.HospitalId, e.Name }).IsUnique();
            });

            // StaffMember configuration
            modelBuilder.Entity<StaffMember>(entity =>
            {
                entity.ToTable("StaffMembers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).HasMaxLength(50);
                entity.Property(e => e.DepartmentId).IsRequired();
                entity.Property(e => e.StaffRole).IsRequired();
                entity.Property(e => e.LicenseNumber).HasMaxLength(50);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // Relationships
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.StaffMembers)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint on UserId
                entity.HasIndex(e => e.UserId).IsUnique();
                
                // Unique constraint on LicenseNumber (if not null)
                entity.HasIndex(e => e.LicenseNumber).IsUnique().HasFilter("\"LicenseNumber\" IS NOT NULL");
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Set CreatedAt and UpdatedAt before saving
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is HealthMonitoring.SharedKernel.DomainModels.BaseEntity entityBase)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entityBase.SetUpdatedAt();
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}