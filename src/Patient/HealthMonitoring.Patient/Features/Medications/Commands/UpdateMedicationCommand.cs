// UpdateMedicationCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Medications.Commands
{
    public class UpdateMedicationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Instructions { get; set; }
    }

    public class UpdateMedicationCommandValidator : AbstractValidator<UpdateMedicationCommand>
    {
        public UpdateMedicationCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Medication ID is required");

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
        }
    }

    public class UpdateMedicationCommandHandler : IRequestHandler<UpdateMedicationCommand, Result>
    {
        private readonly PatientDbContext _context;

        public UpdateMedicationCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateMedicationCommand request, CancellationToken cancellationToken)
        {
            // Find the medication by ID
            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medication == null)
            {
                return Result.Failure($"Medication with ID {request.Id} not found");
            }

            // Update the medication
            medication.UpdateDetails(
                request.Name,
                request.Dosage,
                request.Frequency,
                request.StartDate,
                request.EndDate,
                request.Instructions);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}