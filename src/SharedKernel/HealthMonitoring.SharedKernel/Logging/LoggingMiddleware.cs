using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Contains middleware for logging
    /// </summary>
    public static class LoggingMiddleware
    {
        /// <summary>
        /// Adds correlation ID to all logs via LogContext
        /// </summary>
        public static IApplicationBuilder UseCorrelationIdLogging(this IApplicationBuilder app)
        {
            return app.Use(async (httpContext, next) =>
            {
                if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
                {
                    using (LogContext.PushProperty("CorrelationId", correlationId))
                    {
                        await next();
                    }
                }
                else
                {
                    await next();
                }
            });
        }
    }
}