using System;
using HealthMonitoring.Patient.Features.Devices.Models;
using HealthMonitoring.Patient.Features.MedicalRecords.Models;
using HealthMonitoring.Patient.Features.Medications.Models;
using HealthMonitoring.SharedKernel.DomainModels;

namespace HealthMonitoring.Patient.Features.Patients.Models
{
    /// <summary>
    /// Hasta bilgilerini temsil eden sınıf
    /// </summary>
    public class Patient : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string IdentificationNumber { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        public string BloodType { get; private set; }
        public string Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string EmergencyContactName { get; private set; }
        public string EmergencyContactPhone { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<MedicalRecord> MedicalRecords { get; private set; }
        public virtual ICollection<Medication> Medications { get; private set; }
        public virtual ICollection<PatientDevice> Devices { get; private set; }

        protected Patient() : base()
        {
            MedicalRecords = new List<MedicalRecord>();
            Medications = new List<Medication>();
            Devices = new List<PatientDevice>();
        }

        public Patient(
            Guid userId,
            string firstName,
            string lastName,
            string identificationNumber,
            DateTime dateOfBirth,
            string gender,
            string bloodType,
            string address,
            string phoneNumber,
            string email,
            string emergencyContactName,
            string emergencyContactPhone) : this()
        {
            UserId = userId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            IdentificationNumber = identificationNumber;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            BloodType = bloodType;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            EmergencyContactName = emergencyContactName;
            EmergencyContactPhone = emergencyContactPhone;
            IsActive = true;
        }

        public void UpdatePersonalInfo(
            string firstName,
            string lastName,
            string identificationNumber,
            DateTime dateOfBirth,
            string gender,
            string bloodType,
            string address,
            string phoneNumber,
            string email,
            string emergencyContactName,
            string emergencyContactPhone)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            IdentificationNumber = identificationNumber;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            BloodType = bloodType;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            EmergencyContactName = emergencyContactName;
            EmergencyContactPhone = emergencyContactPhone;
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

        public string FullName => $"{FirstName} {LastName}";

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}