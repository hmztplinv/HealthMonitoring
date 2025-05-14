using System;
using HealthMonitoring.Identity.API.Extensions;
using HealthMonitoring.Identity.Application;
using HealthMonitoring.Identity.Infrastructure;
using HealthMonitoring.Identity.Infrastructure.Data;
using HealthMonitoring.SharedKernel.ErrorHandling;
using HealthMonitoring.SharedKernel.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

try
{
    // Build the application
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("ApplicationName", "HealthMonitoring.Identity"));

    Log.Information("Starting Identity API");

    // Add services to the container
    builder.ConfigureServices();

    // Add action filter for logging controller actions
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<LoggingActionFilter>();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios.
        app.UseHsts();
    }

    // Initialize database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var initializer = services.GetRequiredService<IdentityDbContextInitializer>();
            initializer.InitializeAsync().Wait();
            initializer.SeedAsync().Wait();
            Log.Information("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database");
        }
    }

    // Request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
            
            if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                diagnosticContext.Set("CorrelationId", correlationId);
            }
            
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
            }
        };
    });

    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthMonitoring.Identity API V1");
    });

    app.UseRouting();
    app.UseCors("CorsPolicy");

    // Add correlation ID middleware
    app.UseMiddleware<CorrelationIdMiddleware>();
    
    // Add performance logging middleware
    app.UseMiddleware<PerformanceLoggingMiddleware>();
    
    // Add correlation ID to log context
    app.Use(async (httpContext, next) =>
    {
        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next();
            }
        }
        else
        {
            await next();
        }
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    app.MapControllers();

    app.Run();
    
    Log.Information("Application stopping");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}