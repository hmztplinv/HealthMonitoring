using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.SharedKernel.ErrorHandling;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.SharedKernel.Validation
{
    /// <summary>
    /// MediatR pipeline behavior for validation
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class ValidationBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> HandleAsync(
            TRequest request,
            CancellationToken cancellationToken,
            System.Func<Task<TResponse>> next)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            // Create a context for the request
            var context = new ValidationContext<TRequest>(request);

            // Execute all validators
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Combine the validation failures
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning(
                    "Validation failed for request {RequestType} with {FailureCount} failures",
                    typeof(TRequest).Name,
                    failures.Count);

                // Group validation failures by property name
                var failuresByProperty = failures
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                throw new ErrorHandling.ValidationException(
                    $"One or more validation failures occurred for {typeof(TRequest).Name}",
                    failuresByProperty);
            }

            return await next();
        }
    }
}