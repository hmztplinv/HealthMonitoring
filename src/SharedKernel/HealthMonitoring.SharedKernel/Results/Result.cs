using System.Collections.Generic;
using System.Linq;

namespace HealthMonitoring.SharedKernel.Results
{
    /// <summary>
    /// Represents the result of an operation
    /// </summary>
    public class Result
    {
        private readonly List<string> _errors;

        /// <summary>
        /// Indicates if the operation was successful
        /// </summary>
        public bool IsSuccess => !_errors.Any();
        
        /// <summary>
        /// Indicates if the operation failed
        /// </summary>
        public bool IsFailure => !IsSuccess;
        
        /// <summary>
        /// The errors that occurred during the operation
        /// </summary>
        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        protected Result(List<string> errors)
        {
            _errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Creates a successful result
        /// </summary>
        /// <returns>A successful result</returns>
        public static Result Success()
        {
            return new Result(new List<string>());
        }

        /// <summary>
        /// Creates a failed result with the specified errors
        /// </summary>
        /// <param name="errors">The errors that occurred</param>
        /// <returns>A failed result</returns>
        public static Result Failure(params string[] errors)
        {
            return new Result(errors.ToList());
        }

        /// <summary>
        /// Creates a failed result with a single error
        /// </summary>
        /// <param name="error">The error that occurred</param>
        /// <returns>A failed result</returns>
        public static Result Failure(string error)
        {
            return new Result(new List<string> { error });
        }
    }
}