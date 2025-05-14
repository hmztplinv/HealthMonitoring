using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HealthMonitoring.SharedKernel.ErrorHandling
{
    /// <summary>
    /// Middleware for handling exceptions globally
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string correlationId = context.Items.TryGetValue("CorrelationId", out var corrId) ? corrId.ToString() : "unknown";
                string requestPath = context.Request.Path;
                string requestMethod = context.Request.Method;
                
                // Log the exception with details
                _logger.LogError(ex, 
                    "An unhandled exception occurred during {RequestMethod} {RequestPath} with CorrelationId {CorrelationId}",
                    requestMethod, requestPath, correlationId);
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = exception switch
            {
                NotFoundException _ => HttpStatusCode.NotFound,
                ValidationException _ => HttpStatusCode.BadRequest,
                DomainException _ => HttpStatusCode.BadRequest,
                UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var correlationId = context.Items.TryGetValue("CorrelationId", out var corrId) ? corrId.ToString() : null;

            object response = new
            {
                statusCode = (int)statusCode,
                message = exception.Message,
                correlationId = correlationId,
                exceptionType = exception.GetType().Name,
                stackTrace = context.Request.Headers["X-Debug"] == "true" ? exception.StackTrace : null
            };

            if (exception is ValidationException validationException)
            {
                response = new
                {
                    statusCode = (int)statusCode,
                    message = exception.Message,
                    correlationId = correlationId,
                    exceptionType = exception.GetType().Name,
                    errors = validationException.Errors,
                    stackTrace = context.Request.Headers["X-Debug"] == "true" ? exception.StackTrace : null
                };
            }

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}