using System;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Extensions for structured logging
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs information with correlation ID
        /// </summary>
        public static void LogInformationWithCorrelation(this ILogger logger, string correlationId, string message, params object[] args)
        {
            logger.LogInformation($"[CorrelationId: {correlationId}] {message}", args);
        }

        /// <summary>
        /// Logs an error with correlation ID
        /// </summary>
        public static void LogErrorWithCorrelation(this ILogger logger, string correlationId, Exception exception, string message, params object[] args)
        {
            logger.LogError(exception, $"[CorrelationId: {correlationId}] {message}", args);
        }

        /// <summary>
        /// Logs a warning with correlation ID
        /// </summary>
        public static void LogWarningWithCorrelation(this ILogger logger, string correlationId, string message, params object[] args)
        {
            logger.LogWarning($"[CorrelationId: {correlationId}] {message}", args);
        }
    }
}