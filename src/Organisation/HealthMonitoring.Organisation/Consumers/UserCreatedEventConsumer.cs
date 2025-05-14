// src/Organisation/HealthMonitoring.Organisation/Consumers/UserCreatedEventConsumer.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Staff.Commands;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.EventModels;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Organisation.Consumers
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

            // Kullanıcı rollerini kontrol edelim
            // Eğer Doctor, Nurse veya Assistant rollerinden biri varsa Staff oluşturalım
            var staffRoles = new[] { UserRole.Doctor, UserRole.Nurse, UserRole.Assistant };
            
            // Eğer kullanıcı Organization'ın ilgilendiği bir role sahipse
            if (ShouldCreateStaffMember(userEvent))
            {
                // Departman ID'sini varsayılan olarak alıyoruz - daha sonra atanabilir
                // Gerçek projede bunu kullanıcı arayüzünden veya farklı şekilde alabilirsiniz
                var defaultDepartmentId = await GetDefaultDepartmentId();

                // Staff rolünü belirleyelim
                var staffRole = DetermineStaffRole(userEvent);

                var command = new CreateStaffMemberCommand
                {
                    UserId = userEvent.UserId,
                    FirstName = userEvent.FirstName,
                    LastName = userEvent.LastName,
                    Title = DetermineTitleFromRole(staffRole),
                    DepartmentId = defaultDepartmentId,
                    StaffRole = staffRole,
                    LicenseNumber = "" // Varsayılan olarak boş, daha sonra güncellenebilir
                };

                try
                {
                    var result = await _mediator.Send(command);
                    if (result.IsSuccess)
                    {
                        _logger.LogInformation($"Staff member created for user {userEvent.UserId}");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to create staff member: {string.Join(", ", result.Errors)}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating staff member for user {userEvent.UserId}");
                }
            }
        }

        private bool ShouldCreateStaffMember(UserCreatedEvent userEvent)
        {
            // Rol ID'lerini UserRole enum değerlerine dönüştürmek için bir yöntem gerekli
            // Bu örnekte basitleştiriyoruz - gerçek uygulamada rol ID'lerini çözümlemelisiniz
            return true; // Örnekte herkesi staff olarak ekliyoruz, gerçek uygulamada koşul belirleyin
        }

        private UserRole DetermineStaffRole(UserCreatedEvent userEvent)
        {
            // Burada rol ID'lerinden UserRole belirlemek için bir mantık kurulmalı
            // Şimdilik basit bir örnek:
            return UserRole.Doctor; // Varsayılan olarak
        }

        private string DetermineTitleFromRole(UserRole role)
        {
            return role switch
            {
                UserRole.Doctor => "Dr.",
                UserRole.Nurse => "Hemşire",
                UserRole.Assistant => "Asistan",
                _ => ""
            };
        }

        private async Task<Guid> GetDefaultDepartmentId()
        {
            // Burada varsayılan bir departman ID'si alınmalı
            // Gerçek uygulamada veritabanından sorgulama yapabilirsiniz
            return Guid.Parse("00000000-0000-0000-0000-000000000001"); // Örnek ID
        }
    }
}