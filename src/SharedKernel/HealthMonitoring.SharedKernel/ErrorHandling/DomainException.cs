using System;

namespace HealthMonitoring.SharedKernel.ErrorHandling
{
    /// <summary>
    /// Exception that is thrown when a domain rule is violated
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}