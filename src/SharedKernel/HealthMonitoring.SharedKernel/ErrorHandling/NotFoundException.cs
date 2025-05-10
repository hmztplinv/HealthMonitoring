using System;

namespace HealthMonitoring.SharedKernel.ErrorHandling
{
    /// <summary>
    /// Exception that is thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string entityName, object id) 
            : base($"Entity \"{entityName}\" with id \"{id}\" was not found.")
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}