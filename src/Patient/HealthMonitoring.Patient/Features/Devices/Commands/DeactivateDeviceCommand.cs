// DeactivateDeviceCommand.cs
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
    public class DeactivateDeviceCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateDeviceCommandValidator : AbstractValidator<DeactivateDeviceCommand>
    {
        public DeactivateDeviceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Patient device ID is required");
        }
    }

    public class DeactivateDeviceCommandHandler : IRequestHandler<DeactivateDeviceCommand, Result>
    {
        private readonly PatientDbContext _context;

        public DeactivateDeviceCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeactivateDeviceCommand request, CancellationToken cancellationToken)
        {
            // Find the patient device by ID
            var patientDevice = await _context.PatientDevices
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (patientDevice == null)
            {
                return Result.Failure($"Patient device with ID {request.Id} not found");
            }

            // Deactivate the device
            patientDevice.Deactivate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}