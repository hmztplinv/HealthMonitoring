using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Middleware for managing correlation IDs
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(
            RequestDelegate next,
            ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrCreateCorrelationId(context);
            
            // Add correlation ID to the response headers
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
                return Task.CompletedTask;
            });

            // Store correlation ID in HttpContext.Items
            context.Items["CorrelationId"] = correlationId;
            
            _logger.LogInformation("Request {Method} {Path} started with CorrelationId {CorrelationId}", 
                context.Request.Method, context.Request.Path, correlationId);

            await _next(context);
            
            _logger.LogInformation("Request {Method} {Path} completed with CorrelationId {CorrelationId} and status code {StatusCode}", 
                context.Request.Method, context.Request.Path, correlationId, context.Response.StatusCode);
        }

        private string GetOrCreateCorrelationId(HttpContext context)
        {
            // Try to get correlation ID from the request headers
            if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
            {
                return correlationId;
            }

            // Create a new correlation ID
            return Guid.NewGuid().ToString();
        }
    }
}