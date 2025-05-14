using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Middleware for performance monitoring
    /// </summary>
    public class PerformanceLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceLoggingMiddleware> _logger;
        private readonly Stopwatch _stopwatch;

        public PerformanceLoggingMiddleware(
            RequestDelegate next,
            ILogger<PerformanceLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _stopwatch.Restart();

            await _next(context);

            _stopwatch.Stop();
            
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            var path = context.Request.Path;
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;
            var correlationId = context.Items.TryGetValue("CorrelationId", out var corrId) ? corrId.ToString() : "unknown";

            // Log request completion with timing
            if (elapsedMilliseconds > 500) // Log warning for slow requests
            {
                _logger.LogWarning(
                    "Request {Method} {Path} with CorrelationId {CorrelationId} completed with status code {StatusCode} in {ElapsedMilliseconds}ms (slow)",
                    method, path, correlationId, statusCode, elapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "Request {Method} {Path} with CorrelationId {CorrelationId} completed with status code {StatusCode} in {ElapsedMilliseconds}ms",
                    method, path, correlationId, statusCode, elapsedMilliseconds);
            }
        }
    }
}