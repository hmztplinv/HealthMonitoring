using System;
using System.Threading.Tasks;
using HealthMonitoring.SharedKernel.EventModels;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.SharedKernel.Messaging
{
    /// <summary>
    /// Extensions for the message bus
    /// </summary>
    public static class MessageBusExtensions
    {
        /// <summary>
        /// Publishes an integration event with error handling and logging
        /// </summary>
        /// <param name="publishEndpoint">The publish endpoint</param>
        /// <param name="event">The event to publish</param>
        /// <param name="logger">The logger</param>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <returns>A task that completes when the event is published</returns>
        public static async Task PublishEventWithLogging<TEvent>(
            this IPublishEndpoint publishEndpoint,
            TEvent @event,
            ILogger logger) where TEvent : BaseIntegrationEvent
        {
            try
            {
                logger.LogInformation(
                    "Publishing event {EventType} with ID {EventId}",
                    typeof(TEvent).Name,
                    @event.Id);

                await publishEndpoint.Publish(@event);

                logger.LogInformation(
                    "Published event {EventType} with ID {EventId}",
                    typeof(TEvent).Name,
                    @event.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to publish event {EventType} with ID {EventId}",
                    typeof(TEvent).Name,
                    @event.Id);

                throw;
            }
        }
    }
}