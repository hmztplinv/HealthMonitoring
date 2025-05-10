using System;
using System.Collections.Generic;

namespace HealthMonitoring.SharedKernel.ErrorHandling
{
    /// <summary>
    /// Exception that is thrown when validation fails
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// The validation errors
        /// </summary>
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, IDictionary<string, string[]> errors) : base(message)
        {
            Errors = new Dictionary<string, string[]>(errors);
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }
    }
}