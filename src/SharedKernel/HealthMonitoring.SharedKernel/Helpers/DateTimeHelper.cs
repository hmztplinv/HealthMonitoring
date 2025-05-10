using System;

namespace HealthMonitoring.SharedKernel.Helpers
{
    /// <summary>
    /// Helper class for date-time operations
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts a Unix timestamp to a DateTime
        /// </summary>
        /// <param name="unixTimeStamp">The Unix timestamp (seconds since epoch)</param>
        /// <returns>A DateTime representing the Unix timestamp</returns>
        public static DateTime FromUnixTimeSeconds(long unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(unixTimeStamp);
        }

        /// <summary>
        /// Converts a Unix timestamp (milliseconds) to a DateTime
        /// </summary>
        /// <param name="unixTimeStamp">The Unix timestamp (milliseconds since epoch)</param>
        /// <returns>A DateTime representing the Unix timestamp</returns>
        public static DateTime FromUnixTimeMilliseconds(long unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddMilliseconds(unixTimeStamp);
        }

        /// <summary>
        /// Converts a DateTime to a Unix timestamp (seconds)
        /// </summary>
        /// <param name="dateTime">The DateTime to convert</param>
        /// <returns>The Unix timestamp (seconds since epoch)</returns>
        public static long ToUnixTimeSeconds(DateTime dateTime)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - unixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Converts a DateTime to a Unix timestamp (milliseconds)
        /// </summary>
        /// <param name="dateTime">The DateTime to convert</param>
        /// <returns>The Unix timestamp (milliseconds since epoch)</returns>
        public static long ToUnixTimeMilliseconds(DateTime dateTime)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - unixEpoch).TotalMilliseconds;
        }
    }
}