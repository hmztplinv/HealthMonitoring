using System.Collections.Generic;
using System.Linq;

namespace HealthMonitoring.SharedKernel.Results
{
    /// <summary>
    /// Represents the result of an operation that returns a value
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// The value returned by the operation
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Indicates whether the result has a value
        /// </summary>
        public bool HasValue => Value != null;

        protected Result(T value, List<string> errors) : base(errors)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a successful result with the specified value
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>A successful result containing the value</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(value, new List<string>());
        }

        /// <summary>
        /// Creates a failed result with the specified errors
        /// </summary>
        /// <param name="errors">The errors that occurred</param>
        /// <returns>A failed result</returns>
        public new static Result<T> Failure(params string[] errors)
        {
            return new Result<T>(default, errors.ToList());
        }

        /// <summary>
        /// Creates a failed result with a single error
        /// </summary>
        /// <param name="error">The error that occurred</param>
        /// <returns>A failed result</returns>
        public new static Result<T> Failure(string error)
        {
            return new Result<T>(default, new List<string> { error });
        }
    }
}