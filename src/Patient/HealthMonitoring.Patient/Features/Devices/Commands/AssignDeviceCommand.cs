// AssignDeviceCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Features.Devices.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Devices.Commands
{
    public class AssignDeviceCommand : IRequest<Result<Guid>>
    {
        public Guid PatientId { get; set; }
        public Guid DeviceId { get; set; }
        public string DeviceSerialNumber { get; set; }
        public DeviceType DeviceType { get; set; }
        public string AssignedBy { get; set; }
        public Guid AssignedByUserId { get; set; }
        public string Notes { get; set; }
    }

    public class AssignDeviceCommandValidator : AbstractValidator<AssignDeviceCommand>
    {
        public AssignDeviceCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Device ID is required");

            RuleFor(x => x.DeviceSerialNumber)
                .NotEmpty().WithMessage("Device serial number is required")
                .MaximumLength(50).WithMessage("Device serial number cannot exceed 50 characters");

            RuleFor(x => x.DeviceType)
                .IsInEnum().WithMessage("Invalid device type");

            RuleFor(x => x.AssignedBy)
                .NotEmpty().WithMessage("Assigned by is required")
                .MaximumLength(100).WithMessage("Assigned by cannot exceed 100 characters");

            RuleFor(x => x.AssignedByUserId)
                .NotEmpty().WithMessage("Assigned by user ID is required");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }

    public class AssignDeviceCommandHandler : IRequestHandler<AssignDeviceCommand, Result<Guid>>
    {
        private readonly PatientDbContext _context;

        public AssignDeviceCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(AssignDeviceCommand request, CancellationToken cancellationToken)
        {
            // Check if patient exists
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
            {
                return Result<Guid>.Failure($"Patient with ID {request.PatientId} not found");
            }

            // Check if device is already assigned to this patient
            var deviceAssigned = await _context.PatientDevices
                .AnyAsync(d => d.PatientId == request.PatientId && 
                               d.DeviceId == request.DeviceId && 
                               d.IsActive, 
                         cancellationToken);

            if (deviceAssigned)
            {
                return Result<Guid>.Failure($"Device with ID {request.DeviceId} is already assigned to this patient");
            }

            // Create a new patient device
            var patientDevice = new PatientDevice(
                request.PatientId,
                request.DeviceId,
                request.DeviceSerialNumber,
                request.DeviceType,
                request.AssignedBy,
                request.AssignedByUserId,
                request.Notes);

            // Add to database
            await _context.PatientDevices.AddAsync(patientDevice, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(patientDevice.Id);
        }
    }
}