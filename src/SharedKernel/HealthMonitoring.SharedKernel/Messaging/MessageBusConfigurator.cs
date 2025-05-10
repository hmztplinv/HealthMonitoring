using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace HealthMonitoring.SharedKernel.Messaging
{
    /// <summary>
    /// Configurator for MassTransit message bus
    /// </summary>
    public static class MessageBusConfigurator
    {
        /// <summary>
        /// Configures MassTransit with RabbitMQ
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="hostName">The RabbitMQ host name</param>
        /// <param name="userName">The RabbitMQ user name</param>
        /// <param name="password">The RabbitMQ password</param>
        /// <param name="virtualHost">The RabbitMQ virtual host</param>
        /// <param name="configureBus">Additional bus configuration</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection ConfigureMessageBus(
            this IServiceCollection services,
            string hostName,
            string userName,
            string password,
            string virtualHost = "/",
            Action<IBusRegistrationConfigurator> configureBus = null)
        {
            services.AddMassTransit(busConfig =>
            {
                configureBus?.Invoke(busConfig);

                busConfig.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(hostName, virtualHost, h =>
                    {
                        h.Username(userName);
                        h.Password(password);
                    });

                    // Configure retry policy
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    // Configure error handling
                    cfg.UseDelayedRedelivery(r => r.Intervals(
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromMinutes(15)));

                    cfg.UseInMemoryOutbox(context);
                    
                    // Configure the endpoints by convention
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}