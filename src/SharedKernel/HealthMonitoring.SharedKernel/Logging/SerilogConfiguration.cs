using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace HealthMonitoring.SharedKernel.Logging
{
    /// <summary>
    /// Provides centralized Serilog configuration for all microservices
    /// </summary>
    public static class SerilogConfiguration
    {
        // Log dosyası için merkezi dizin
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        /// <summary>
        /// Configures the application to use Serilog
        /// </summary>
        public static WebApplicationBuilder ConfigureSerilog(
            this WebApplicationBuilder builder, 
            string applicationName)
        {
            // Log dosyası dizinini oluşturma
            Directory.CreateDirectory(LogDirectory);
            var logFilePath = Path.Combine(LogDirectory, $"{applicationName}-.log");
            
            // Create bootstrap logger for startup
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateBootstrapLogger();

            // Yapılandırmada log dosyası ayarı olmasa bile varsayılan dosyayı kullanma
            builder.Host.UseSerilog((context, services, configuration) => 
            {
                var serilogConfig = configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty("ApplicationName", applicationName);

                // Konfigürasyonda file sink yoksa varsayılanı ekle
                serilogConfig.WriteTo.File(logFilePath, 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
            });

            return builder;
        }

        /// <summary>
        /// Configures the HTTP request pipeline with Serilog middleware
        /// </summary>
        public static WebApplication ConfigureSerilogRequestLogging(this WebApplication app)
        {
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

            return app;
        }

        /// <summary>
        /// Wraps the main application execution with Serilog exception handling
        /// </summary>
        public static void RunWithSerilog(this WebApplication app, Action<WebApplication> runAction)
        {
            try
            {
                Log.Information("Starting web application");
                Log.Information("Log dosyası konumu: {LogDirectory}", LogDirectory);
                runAction(app);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}