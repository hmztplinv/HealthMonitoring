using System;
using HealthMonitoring.SharedKernel.DomainModels;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Patient.Features.Devices.Models
{
    /// <summary>
    /// Hasta-cihaz ilişkisini temsil eden sınıf
    /// </summary>
    public class PatientDevice : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid DeviceId { get; private set; }
        public string DeviceSerialNumber { get; private set; }
        public DeviceType DeviceType { get; private set; }
        public DateTime AssignedDate { get; private set; }
        public DateTime? UnassignedDate { get; private set; }
        public string AssignedBy { get; private set; }
        public Guid AssignedByUserId { get; private set; }
        public bool IsActive { get; private set; }
        public string Notes { get; private set; }

        // Navigation properties
        public virtual Patient.Features.Patients.Models.Patient Patient { get; private set; }

        protected PatientDevice() : base()
        {
        }

        public PatientDevice(
            Guid patientId,
            Guid deviceId,
            string deviceSerialNumber,
            DeviceType deviceType,
            string assignedBy,
            Guid assignedByUserId,
            string notes) : this()
        {
            PatientId = patientId;
            DeviceId = deviceId;
            DeviceSerialNumber = deviceSerialNumber ?? throw new ArgumentNullException(nameof(deviceSerialNumber));
            DeviceType = deviceType;
            AssignedDate = DateTime.UtcNow;
            AssignedBy = assignedBy ?? throw new ArgumentNullException(nameof(assignedBy));
            AssignedByUserId = assignedByUserId;
            IsActive = true;
            Notes = notes;
        }

        public void Unassign(DateTime unassignDate)
        {
            UnassignedDate = unassignDate;
            IsActive = false;
            SetUpdatedAt();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
            SetUpdatedAt();
        }

        public void Activate()
        {
            IsActive = true;
            UnassignedDate = null;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            if (UnassignedDate == null)
            {
                UnassignedDate = DateTime.UtcNow;
            }
            SetUpdatedAt();
        }
    }
}