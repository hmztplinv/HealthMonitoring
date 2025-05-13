// CreateMedicalRecordCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Features.MedicalRecords.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Commands
{
    public class CreateMedicalRecordCommand : IRequest<Result<Guid>>
    {
        public Guid PatientId { get; set; }
        public string RecordType { get; set; }
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public Guid RecordedByUserId { get; set; }
        public string RecordedByName { get; set; }
    }

    public class CreateMedicalRecordCommandValidator : AbstractValidator<CreateMedicalRecordCommand>
    {
        public CreateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required");

            RuleFor(x => x.RecordType)
                .NotEmpty().WithMessage("Record type is required")
                .MaximumLength(50).WithMessage("Record type cannot exceed 50 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Diagnosis)
                .MaximumLength(500).WithMessage("Diagnosis cannot exceed 500 characters");

            RuleFor(x => x.Treatment)
                .MaximumLength(500).WithMessage("Treatment cannot exceed 500 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

            RuleFor(x => x.RecordedByUserId)
                .NotEmpty().WithMessage("Recorded by user ID is required");

            RuleFor(x => x.RecordedByName)
                .NotEmpty().WithMessage("Recorded by name is required")
                .MaximumLength(100).WithMessage("Recorded by name cannot exceed 100 characters");
        }
    }

    public class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, Result<Guid>>
    {
        private readonly PatientDbContext _context;

        public CreateMedicalRecordCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            // Check if patient exists
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
            {
                return Result<Guid>.Failure($"Patient with ID {request.PatientId} not found");
            }

            // Create a new medical record
            var medicalRecord = new MedicalRecord(
                request.PatientId,
                request.RecordType,
                request.Description,
                request.Diagnosis,
                request.Treatment,
                request.Notes,
                request.RecordedByUserId,
                request.RecordedByName);

            // Add to database
            await _context.MedicalRecords.AddAsync(medicalRecord, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(medicalRecord.Id);
        }
    }
}