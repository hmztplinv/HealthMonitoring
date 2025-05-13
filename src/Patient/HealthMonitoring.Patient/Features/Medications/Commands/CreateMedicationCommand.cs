// CreateMedicationCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Features.Medications.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Medications.Commands
{
    public class CreateMedicationCommand : IRequest<Result<Guid>>
    {
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Instructions { get; set; }
        public string PrescribedBy { get; set; }
        public Guid PrescribedByUserId { get; set; }
    }

    public class CreateMedicationCommandValidator : AbstractValidator<CreateMedicationCommand>
    {
        public CreateMedicationCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Medication name is required")
                .MaximumLength(100).WithMessage("Medication name cannot exceed 100 characters");

            RuleFor(x => x.Dosage)
                .NotEmpty().WithMessage("Dosage is required")
                .MaximumLength(50).WithMessage("Dosage cannot exceed 50 characters");

            RuleFor(x => x.Frequency)
                .NotEmpty().WithMessage("Frequency is required")
                .MaximumLength(50).WithMessage("Frequency cannot exceed 50 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Instructions)
                .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters");

            RuleFor(x => x.PrescribedBy)
                .NotEmpty().WithMessage("Prescribed by is required")
                .MaximumLength(100).WithMessage("Prescribed by cannot exceed 100 characters");

            RuleFor(x => x.PrescribedByUserId)
                .NotEmpty().WithMessage("Prescribed by user ID is required");
        }
    }

    public class CreateMedicationCommandHandler : IRequestHandler<CreateMedicationCommand, Result<Guid>>
    {
        private readonly PatientDbContext _context;

        public CreateMedicationCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateMedicationCommand request, CancellationToken cancellationToken)
        {
            // Check if patient exists
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

            if (!patientExists)
            {
                return Result<Guid>.Failure($"Patient with ID {request.PatientId} not found");
            }

            // Create a new medication
            var medication = new Medication(
                request.PatientId,
                request.Name,
                request.Dosage,
                request.Frequency,
                request.StartDate,
                request.EndDate,
                request.Instructions,
                request.PrescribedBy,
                request.PrescribedByUserId);

            // Add to database
            await _context.Medications.AddAsync(medication, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(medication.Id);
        }
    }
}