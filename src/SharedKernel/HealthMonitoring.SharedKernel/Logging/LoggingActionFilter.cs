using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Action filter for logging controller actions
    /// </summary>
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var correlationId = context.HttpContext.Items.TryGetValue("CorrelationId", out var corrId) ? corrId.ToString() : "unknown";
            
            _logger.LogInformation(
                "Starting action {ControllerName}.{ActionName} with CorrelationId {CorrelationId}",
                controllerName, actionName, correlationId);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var correlationId = context.HttpContext.Items.TryGetValue("CorrelationId", out var corrId) ? corrId.ToString() : "unknown";
            
            if (context.Exception != null)
            {
                _logger.LogError(context.Exception,
                    "Action {ControllerName}.{ActionName} with CorrelationId {CorrelationId} threw an exception",
                    controllerName, actionName, correlationId);
            }
            else
            {
                var statusCode = context.HttpContext.Response.StatusCode;
                
                _logger.LogInformation(
                    "Completed action {ControllerName}.{ActionName} with CorrelationId {CorrelationId} with status code {StatusCode}",
                    controllerName, actionName, correlationId, statusCode);
            }
        }
    }
}