// UpdateDeviceNotesCommand.cs
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
    public class UpdateDeviceNotesCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateDeviceNotesCommandValidator : AbstractValidator<UpdateDeviceNotesCommand>
    {
        public UpdateDeviceNotesCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Patient device ID is required");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }

    public class UpdateDeviceNotesCommandHandler : IRequestHandler<UpdateDeviceNotesCommand, Result>
    {
        private readonly PatientDbContext _context;

        public UpdateDeviceNotesCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateDeviceNotesCommand request, CancellationToken cancellationToken)
        {
            // Find the patient device by ID
            var patientDevice = await _context.PatientDevices
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (patientDevice == null)
            {
                return Result.Failure($"Patient device with ID {request.Id} not found");
            }

            // Update notes
            patientDevice.UpdateNotes(request.Notes);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}