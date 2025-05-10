using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Identity.Infrastructure.Data
{
    /// <summary>
    /// EF Core DbContext for Identity microservice
    /// </summary>
    public class IdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PasswordSalt).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.LastLoginDate);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // Unique constraint on UserName
                entity.HasIndex(e => e.UserName).IsUnique();
                
                // Unique constraint on Email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.RoleType).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // Unique constraint on Name
                entity.HasIndex(e => e.Name).IsUnique();
                
                // Unique constraint on RoleType
                entity.HasIndex(e => e.RoleType).IsUnique();
            });

            // UserRoleMapping configuration
            modelBuilder.Entity<UserRoleMapping>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.RoleId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);

                // Unique constraint on UserId, RoleId
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();

                // Relationships
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoleMappings)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
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