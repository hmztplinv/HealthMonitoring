using System;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.SharedKernel.EventModels
{
    /// <summary>
    /// Event raised when new vital data is received from a device
    /// </summary>
    public class VitalDataReceivedEvent : BaseIntegrationEvent
    {
        /// <summary>
        /// The patient ID associated with the vital data
        /// </summary>
        public Guid PatientId { get; }
        
        /// <summary>
        /// The device ID that sent the data
        /// </summary>
        public Guid DeviceId { get; }
        
        /// <summary>
        /// The type of vital sign
        /// </summary>
        public VitalType VitalType { get; }
        
        /// <summary>
        /// The value of the vital sign
        /// </summary>
        public decimal Value { get; }
        
        /// <summary>
        /// The timestamp when the data was recorded
        /// </summary>
        public DateTime Timestamp { get; }

        public VitalDataReceivedEvent(
            Guid patientId,
            Guid deviceId,
            VitalType vitalType,
            decimal value,
            DateTime timestamp)
        {
            PatientId = patientId;
            DeviceId = deviceId;
            VitalType = vitalType;
            Value = value;
            Timestamp = timestamp;
        }
    }
}