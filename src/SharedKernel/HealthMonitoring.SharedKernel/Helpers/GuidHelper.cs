using System;

namespace HealthMonitoring.SharedKernel.Helpers
{
    /// <summary>
    /// Helper class for GUID operations
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Generates a sequential GUID
        /// </summary>
        /// <returns>A sequential GUID</returns>
        public static Guid GenerateSequentialGuid()
        {
            // Get current timestamp
            byte[] timestampBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

            // Create a new GUID
            byte[] guidBytes = Guid.NewGuid().ToByteArray();

            // Copy the timestamp into the first 8 bytes of the GUID
            Array.Copy(timestampBytes, guidBytes, Math.Min(timestampBytes.Length, 8));

            return new Guid(guidBytes);
        }

        /// <summary>
        /// Parses a GUID from a string, throwing an exception if parsing fails
        /// </summary>
        /// <param name="guidString">The GUID string</param>
        /// <returns>The parsed GUID</returns>
        public static Guid ParseGuid(string guidString)
        {
            if (string.IsNullOrEmpty(guidString))
            {
                throw new ArgumentException("GUID string cannot be null or empty.", nameof(guidString));
            }

            if (!Guid.TryParse(guidString, out var result))
            {
                throw new FormatException($"String '{guidString}' is not a valid GUID.");
            }

            return result;
        }

        /// <summary>
        /// Tries to parse a GUID from a string
        /// </summary>
        /// <param name="guidString">The GUID string</param>
        /// <param name="result">The parsed GUID if successful</param>
        /// <returns>True if parsing was successful, false otherwise</returns>
        public static bool TryParseGuid(string guidString, out Guid result)
        {
            result = Guid.Empty;

            if (string.IsNullOrEmpty(guidString))
            {
                return false;
            }

            return Guid.TryParse(guidString, out result);
        }
    }
}