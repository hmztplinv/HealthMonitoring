// UpdateMedicalRecordCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Commands
{
    public class UpdateMedicalRecordCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string RecordType { get; set; }
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateMedicalRecordCommandValidator : AbstractValidator<UpdateMedicalRecordCommand>
    {
        public UpdateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Medical record ID is required");

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
        }
    }

    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, Result>
    {
        private readonly PatientDbContext _context;

        public UpdateMedicalRecordCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            // Find the medical record by ID
            var medicalRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medicalRecord == null)
            {
                return Result.Failure($"Medical record with ID {request.Id} not found");
            }

            // Update the medical record
            medicalRecord.UpdateRecord(
                request.RecordType,
                request.Description,
                request.Diagnosis,
                request.Treatment,
                request.Notes);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}