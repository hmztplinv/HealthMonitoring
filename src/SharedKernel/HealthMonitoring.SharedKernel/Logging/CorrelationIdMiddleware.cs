using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Middleware for managing correlation IDs
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
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

            await _next(context);
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