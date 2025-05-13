// UnassignDeviceCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Devices.Commands
{
    public class UnassignDeviceCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DateTime UnassignDate { get; set; }
    }

    public class UnassignDeviceCommandValidator : AbstractValidator<UnassignDeviceCommand>
    {
        public UnassignDeviceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Patient device ID is required");

            RuleFor(x => x.UnassignDate)
                .NotEmpty().WithMessage("Unassign date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Unassign date cannot be in the future");
        }
    }

    public class UnassignDeviceCommandHandler : IRequestHandler<UnassignDeviceCommand, Result>
    {
        private readonly PatientDbContext _context;

        public UnassignDeviceCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UnassignDeviceCommand request, CancellationToken cancellationToken)
        {
            // Find the patient device by ID
            var patientDevice = await _context.PatientDevices
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (patientDevice == null)
            {
                return Result.Failure($"Patient device with ID {request.Id} not found");
            }

            // Unassign the device
            patientDevice.Unassign(request.UnassignDate);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}