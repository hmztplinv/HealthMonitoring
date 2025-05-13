using System;
using HealthMonitoring.SharedKernel.DomainModels;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Models
{
    /// <summary>
    /// Hastanın tıbbi kaydını temsil eden sınıf
    /// </summary>
    public class MedicalRecord : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public DateTime RecordDate { get; private set; }
        public string RecordType { get; private set; }
        public string Description { get; private set; }
        public string Diagnosis { get; private set; }
        public string Treatment { get; private set; }
        public string Notes { get; private set; }
        public Guid RecordedByUserId { get; private set; }
        public string RecordedByName { get; private set; }

        // Navigation properties
        public virtual Patient.Features.Patients.Models.Patient Patient { get; private set; }

        protected MedicalRecord() : base()
        {
        }

        public MedicalRecord(
            Guid patientId,
            string recordType,
            string description,
            string diagnosis,
            string treatment,
            string notes,
            Guid recordedByUserId,
            string recordedByName) : this()
        {
            PatientId = patientId;
            RecordDate = DateTime.UtcNow;
            RecordType = recordType ?? throw new ArgumentNullException(nameof(recordType));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Diagnosis = diagnosis;
            Treatment = treatment;
            Notes = notes;
            RecordedByUserId = recordedByUserId;
            RecordedByName = recordedByName ?? throw new ArgumentNullException(nameof(recordedByName));
        }

        public void UpdateRecord(
            string recordType,
            string description,
            string diagnosis,
            string treatment,
            string notes)
        {
            RecordType = recordType ?? throw new ArgumentNullException(nameof(recordType));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Diagnosis = diagnosis;
            Treatment = treatment;
            Notes = notes;
            SetUpdatedAt();
        }
    }
}