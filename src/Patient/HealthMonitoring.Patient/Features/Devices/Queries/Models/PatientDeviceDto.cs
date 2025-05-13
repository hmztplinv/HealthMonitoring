// PatientDeviceDto.cs
using System;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.Patient.Features.Devices.Queries.Models
{
    public class PatientDeviceDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid DeviceId { get; set; }
        public string DeviceSerialNumber { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceTypeName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? UnassignedDate { get; set; }
        public string AssignedBy { get; set; }
        public Guid AssignedByUserId { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}