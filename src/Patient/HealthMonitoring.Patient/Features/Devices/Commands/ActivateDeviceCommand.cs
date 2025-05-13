// ActivateDeviceCommand.cs
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
    public class ActivateDeviceCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActivateDeviceCommandValidator : AbstractValidator<ActivateDeviceCommand>
    {
        public ActivateDeviceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Patient device ID is required");
        }
    }

    public class ActivateDeviceCommandHandler : IRequestHandler<ActivateDeviceCommand, Result>
    {
        private readonly PatientDbContext _context;

        public ActivateDeviceCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ActivateDeviceCommand request, CancellationToken cancellationToken)
        {
            // Find the patient device by ID
            var patientDevice = await _context.PatientDevices
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (patientDevice == null)
            {
                return Result.Failure($"Patient device with ID {request.Id} not found");
            }

            // Activate the device
            patientDevice.Activate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}