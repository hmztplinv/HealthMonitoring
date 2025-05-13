using HealthMonitoring.Patient.Features.Patients.Models;
using HealthMonitoring.Patient.Features.MedicalRecords.Models;
using HealthMonitoring.Patient.Features.Medications.Models;
using HealthMonitoring.Patient.Features.Devices.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Infrastructure.Data
{
    /// <summary>
    /// Patient mikroservisi için EF Core DbContext
    /// </summary>
    public class PatientDbContext : DbContext
    {
        public DbSet<Patient.Features.Patients.Models.Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PatientDevice> PatientDevices { get; set; }

        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Patient konfigürasyonu
            modelBuilder.Entity<Patient.Features.Patients.Models.Patient>(entity =>
            {
                entity.ToTable("Patients");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IdentificationNumber).HasMaxLength(20);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.BloodType).HasMaxLength(5);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // UserId için unique constraint
                entity.HasIndex(e => e.UserId).IsUnique();
                
                // IdentificationNumber için unique constraint (eğer boş değilse)
                entity.HasIndex(e => e.IdentificationNumber).IsUnique()
                    .HasFilter("\"IdentificationNumber\" IS NOT NULL");
            });

            // MedicalRecord konfigürasyonu
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.ToTable("MedicalRecords");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PatientId).IsRequired();
                entity.Property(e => e.RecordDate).IsRequired();
                entity.Property(e => e.RecordType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Diagnosis).HasMaxLength(500);
                entity.Property(e => e.Treatment).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.RecordedByUserId).IsRequired();
                entity.Property(e => e.RecordedByName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // İlişkiler
                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.MedicalRecords)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Medication konfigürasyonu
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.ToTable("Medications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PatientId).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Dosage).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Frequency).IsRequired().HasMaxLength(50);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate);
                entity.Property(e => e.Instructions).HasMaxLength(500);
                entity.Property(e => e.PrescribedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrescribedByUserId).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // İlişkiler
                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.Medications)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PatientDevice konfigürasyonu
            modelBuilder.Entity<PatientDevice>(entity =>
            {
                entity.ToTable("PatientDevices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PatientId).IsRequired();
                entity.Property(e => e.DeviceId).IsRequired();
                entity.Property(e => e.DeviceSerialNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DeviceType).IsRequired();
                entity.Property(e => e.AssignedDate).IsRequired();
                entity.Property(e => e.UnassignedDate);
                entity.Property(e => e.AssignedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AssignedByUserId).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                
                // İlişkiler
                entity.HasOne(e => e.Patient)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Unique constraint - Aynı hasta için aynı cihaz tekrar atanamamalı (eğer aktifse)
                entity.HasIndex(e => new { e.PatientId, e.DeviceId })
                    .IsUnique()
                    .HasFilter("\"IsActive\" = true");
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Kaydetmeden önce CreatedAt ve UpdatedAt alanlarını ayarla
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