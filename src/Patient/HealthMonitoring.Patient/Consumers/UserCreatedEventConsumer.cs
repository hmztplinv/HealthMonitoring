// src/Patient/HealthMonitoring.Patient/Consumers/UserCreatedEventConsumer.cs

using System;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Patients.Commands;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.EventModels;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Patient.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserCreatedEventConsumer> _logger;

        public UserCreatedEventConsumer(
            IMediator mediator,
            ILogger<UserCreatedEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var userEvent = context.Message;
            _logger.LogInformation($"Consuming UserCreatedEvent for user {userEvent.UserId}");

            // Kullanıcının Patient olup olmadığını belirle
            if (ShouldCreatePatient(userEvent))
            {
                var command = new CreatePatientCommand
                {
                    UserId = userEvent.UserId,
                    FirstName = userEvent.FirstName,
                    LastName = userEvent.LastName,
                    IdentificationNumber = "", // Boş olarak bırakılıyor, daha sonra güncellenebilir
                    DateOfBirth = DateTime.UtcNow.AddYears(-30), // Varsayılan bir değer
                    Gender = "", // Boş olarak bırakılıyor
                    BloodType = "", // Boş olarak bırakılıyor
                    Address = "", // Boş olarak bırakılıyor
                    PhoneNumber = "", // Boş olarak bırakılıyor
                    Email = userEvent.Email,
                    EmergencyContactName = "", // Boş olarak bırakılıyor
                    EmergencyContactPhone = "" // Boş olarak bırakılıyor
                };

                try
                {
                    var result = await _mediator.Send(command);
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation($"Patient created for user {userEvent.UserId}");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to create patient: {string.Join(", ", result.Errors)}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating patient for user {userEvent.UserId}");
                }
            }
        }

        private bool ShouldCreatePatient(UserCreatedEvent userEvent)
        {
            // Kullanıcının hasta olup olmadığını belirlemek için roller kontrol edilebilir
            // Burada basit bir örnek veriyoruz, gerçek uygulamada daha karmaşık olabilir
            
            // Örnek: UserRole.Patient rolüne sahip kullanıcılar için Patient oluştur
            return HasPatientRole(userEvent);
        }

        private bool HasPatientRole(UserCreatedEvent userEvent)
        {
            // Burada rol ID'lerini kontrol etmek ve Patient rolünü belirlemek için bir mantık gerekli
            // Basit bir örnek olarak daima false dönüyoruz
            return false; // Gerçek uygulamada bu mantığı implemente etmelisiniz
        }
    }
}