using System;
using HealthMonitoring.SharedKernel.DomainModels;

namespace HealthMonitoring.Patient.Features.Medications.Models
{
    /// <summary>
    /// Hastanın ilaç bilgilerini temsil eden sınıf
    /// </summary>
    public class Medication : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public string Name { get; private set; }
        public string Dosage { get; private set; }
        public string Frequency { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string Instructions { get; private set; }
        public string PrescribedBy { get; private set; }
        public Guid PrescribedByUserId { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual Patient.Features.Patients.Models.Patient Patient { get; private set; }

        protected Medication() : base()
        {
        }

        public Medication(
            Guid patientId,
            string name,
            string dosage,
            string frequency,
            DateTime startDate,
            DateTime? endDate,
            string instructions,
            string prescribedBy,
            Guid prescribedByUserId) : this()
        {
            PatientId = patientId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage));
            Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
            StartDate = startDate;
            EndDate = endDate;
            Instructions = instructions;
            PrescribedBy = prescribedBy ?? throw new ArgumentNullException(nameof(prescribedBy));
            PrescribedByUserId = prescribedByUserId;
            IsActive = true;
        }

        public void UpdateDetails(
            string name,
            string dosage,
            string frequency,
            DateTime startDate,
            DateTime? endDate,
            string instructions)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage));
            Frequency = frequency ?? throw new ArgumentNullException(nameof(frequency));
            StartDate = startDate;
            EndDate = endDate;
            Instructions = instructions;
            SetUpdatedAt();
        }

        public void Discontinue(DateTime discontinueDate)
        {
            if (EndDate == null || discontinueDate < EndDate)
            {
                EndDate = discontinueDate;
            }
            
            IsActive = false;
            SetUpdatedAt();
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }
    }
}