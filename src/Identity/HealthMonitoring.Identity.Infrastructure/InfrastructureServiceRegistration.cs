using HealthMonitoring.Identity.Domain.Repositories;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.Identity.Infrastructure.Data;
using HealthMonitoring.Identity.Infrastructure.Repositories;
using HealthMonitoring.Identity.Infrastructure.Services;
using HealthMonitoring.SharedKernel.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthMonitoring.Identity.Infrastructure
{
    /// <summary>
    /// Extension methods for registering infrastructure services
    /// </summary>
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("IdentityDatabase"),
                    b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Register DbContext initializer
            services.AddScoped<IdentityDbContextInitializer>();

            // Configure MassTransit
            services.ConfigureMessageBus(
                configuration["MessageBus:Host"],
                configuration["MessageBus:Username"],
                configuration["MessageBus:Password"],
                configuration["MessageBus:VirtualHost"]);

            return services;
        }
    }
}