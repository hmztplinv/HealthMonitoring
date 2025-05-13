// MedicalRecordDto.cs
using System;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Queries.Models
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; }
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public Guid RecordedByUserId { get; set; }
        public string RecordedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}