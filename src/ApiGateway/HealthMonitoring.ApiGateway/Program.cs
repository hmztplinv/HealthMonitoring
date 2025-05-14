using System.Text;
using HealthMonitoring.ApiGateway.Saga;
using HealthMonitoring.ApiGateway.Saga.Steps;
using HealthMonitoring.SharedKernel.Authentication;
using HealthMonitoring.SharedKernel.ErrorHandling;
using HealthMonitoring.SharedKernel.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
        .Enrich.WithProperty("ApplicationName", "HealthMonitoring.ApiGateway"));

    Log.Information("Starting API Gateway");

    // CORS politikası ekle
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
        });
    });

    // Controller'ları ekle
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<LoggingActionFilter>();
    });

    // YARP Reverse Proxy Konfigürasyonu
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // JWT Authentication Konfigürasyonu
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var key = jwtSettings["Key"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = JwtTokenHelper.GetTokenValidationParameters(key, issuer, audience);
        });

    builder.Services.AddAuthorization();

    // HttpClient Factory
    builder.Services.AddHttpClient("IdentityService", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Identity"]);
    });

    builder.Services.AddHttpClient("OrganisationService", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Organisation"]);
    });

    builder.Services.AddHttpClient("PatientService", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Patient"]);
    });

    // Saga services
    builder.Services.AddScoped<ISagaCoordinator, SagaCoordinator>();
    builder.Services.AddScoped<CreateUserWithRoleSaga>();
    builder.Services.AddScoped<CreatePatientWithUserSaga>();
    builder.Services.AddScoped<CreateUserStep>();
    builder.Services.AddScoped<CreatePatientUserStep>(); // Yeni eklenen adım
    builder.Services.AddScoped<CreateStaffMemberStep>();
    builder.Services.AddScoped<CreatePatientStep>();

    // Swagger/OpenAPI Konfigürasyonu
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthMonitoring API Gateway", Version = "v1" });
        
        // JWT Authentication için Swagger UI konfigürasyonu
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

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

    // HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthMonitoring API Gateway v1"));
    }

    app.UseHttpsRedirection();
    app.UseCors("CorsPolicy");

    // Correlation ID middleware
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

    // Global exception handler
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    app.MapControllers();
    app.MapReverseProxy();

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