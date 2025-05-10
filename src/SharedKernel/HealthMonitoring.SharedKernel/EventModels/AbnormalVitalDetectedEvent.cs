using System;
using HealthMonitoring.SharedKernel.DomainModels.Enums;

namespace HealthMonitoring.SharedKernel.EventModels
{
    /// <summary>
    /// Event raised when an abnormal vital sign is detected
    /// </summary>
    public class AbnormalVitalDetectedEvent : BaseIntegrationEvent
    {
        /// <summary>
        /// The patient ID associated with the abnormal vital
        /// </summary>
        public Guid PatientId { get; }
        
        /// <summary>
        /// The type of vital sign
        /// </summary>
        public VitalType VitalType { get; }
        
        /// <summary>
        /// The value of the vital sign
        /// </summary>
        public decimal Value { get; }
        
        /// <summary>
        /// The threshold value that was exceeded
        /// </summary>
        public decimal ThresholdValue { get; }
        
        /// <summary>
        /// The alert level for this abnormal vital
        /// </summary>
        public AlertLevel AlertLevel { get; }
        
        /// <summary>
        /// The message describing the alert
        /// </summary>
        public string Message { get; }

        public AbnormalVitalDetectedEvent(
            Guid patientId,
            VitalType vitalType,
            decimal value,
            decimal thresholdValue,
            AlertLevel alertLevel,
            string message)
        {
            PatientId = patientId;
            VitalType = vitalType;
            Value = value;
            ThresholdValue = thresholdValue;
            AlertLevel = alertLevel;
            Message = message;
        }
    }
}