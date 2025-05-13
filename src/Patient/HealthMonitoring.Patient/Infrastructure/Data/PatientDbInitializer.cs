using System;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Patients.Models;
using HealthMonitoring.Patient.Features.MedicalRecords.Models;
using HealthMonitoring.Patient.Features.Medications.Models;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Patient.Infrastructure.Data
{
    /// <summary>
    /// Patient mikroservisi için örnek veri oluşturucu
    /// </summary>
    public class PatientDbInitializer
    {
        private readonly PatientDbContext _context;
        private readonly ILogger<PatientDbInitializer> _logger;

        public PatientDbInitializer(
            PatientDbContext context,
            ILogger<PatientDbInitializer> logger)
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
                _logger.LogError(ex, "Veritabanı başlatılırken bir hata oluştu.");
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
                _logger.LogError(ex, "Örnek veri oluşturulurken bir hata oluştu.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // Örnek hasta verileri oluştur (eğer hiç hasta yoksa)
            if (!await _context.Patients.AnyAsync())
            {
                // Örnek hasta oluştur
                var patient1 = new Patient.Features.Patients.Models.Patient(
                    Guid.Parse("00000000-0000-0000-0000-000000000001"), // Bu gerçek ortamda var olan bir user ID olmalı
                    "Ahmet",
                    "Yılmaz",
                    "12345678901",
                    new DateTime(1980, 5, 15),
                    "Erkek",
                    "A Rh+",
                    "İstanbul, Türkiye",
                    "5551234567",
                    "ahmet.yilmaz@example.com",
                    "Ayşe Yılmaz",
                    "5559876543");

                var patient2 = new Patient.Features.Patients.Models.Patient(
                    Guid.Parse("00000000-0000-0000-0000-000000000002"), // Bu gerçek ortamda var olan bir user ID olmalı
                    "Fatma",
                    "Demir",
                    "98765432109",
                    new DateTime(1975, 10, 20),
                    "Kadın",
                    "0 Rh-",
                    "Ankara, Türkiye",
                    "5551112233",
                    "fatma.demir@example.com",
                    "Mehmet Demir",
                    "5554445566");

                await _context.Patients.AddRangeAsync(patient1, patient2);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Örnek hastalar başarıyla oluşturuldu.");

                // Örnek tıbbi kayıtlar oluştur
                var medicalRecord1 = new MedicalRecord(
                    patient1.Id,
                    "Muayene",
                    "Rutin sağlık kontrolü",
                    "Sağlıklı",
                    "İlaç kullanımı gerekmemektedir",
                    "Hasta genel olarak sağlıklı durumda",
                    Guid.Parse("00000000-0000-0000-0000-000000000003"), // Doktor ID
                    "Dr. Mehmet Öz");

                var medicalRecord2 = new MedicalRecord(
                    patient2.Id,
                    "Muayene",
                    "Grip şikayeti",
                    "Üst solunum yolu enfeksiyonu",
                    "Parasetamol 500mg, günde 3 kez",
                    "Hasta 3 gün sonra kontrol edilecek",
                    Guid.Parse("00000000-0000-0000-0000-000000000003"), // Doktor ID
                    "Dr. Mehmet Öz");

                await _context.MedicalRecords.AddRangeAsync(medicalRecord1, medicalRecord2);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Örnek tıbbi kayıtlar başarıyla oluşturuldu.");

                // Örnek ilaç kayıtları oluştur
                var medication1 = new Medication(
                    patient2.Id,
                    "Parol",
                    "500mg",
                    "Günde 3 kez",
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(5),
                    "Yemeklerden sonra alınmalıdır",
                    "Dr. Mehmet Öz",
                    Guid.Parse("00000000-0000-0000-0000-000000000003")); // Doktor ID

                await _context.Medications.AddAsync(medication1);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Örnek ilaç kayıtları başarıyla oluşturuldu.");

                // Örnek cihaz kayıtları oluştur
                var patientDevice1 = new Features.Devices.Models.PatientDevice(
                    patient1.Id,
                    Guid.NewGuid(), // Gerçek ortamda var olan bir cihaz ID olmalı
                    "SWATCH12345",
                    DeviceType.Smartwatch,
                    "Dr. Mehmet Öz",
                    Guid.Parse("00000000-0000-0000-0000-000000000003"), // Doktor ID
                    "Kalp atış hızı takibi için");

                await _context.PatientDevices.AddAsync(patientDevice1);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Örnek cihaz kayıtları başarıyla oluşturuldu.");
            }
        }
    }
}