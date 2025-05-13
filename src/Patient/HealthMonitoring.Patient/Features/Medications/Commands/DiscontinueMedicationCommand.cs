// DiscontinueMedicationCommand.cs
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
    public class DiscontinueMedicationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public DateTime DiscontinueDate { get; set; }
    }

    public class DiscontinueMedicationCommandValidator : AbstractValidator<DiscontinueMedicationCommand>
    {
        public DiscontinueMedicationCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Medication ID is required");

            RuleFor(x => x.DiscontinueDate)
                .NotEmpty().WithMessage("Discontinue date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Discontinue date cannot be in the future");
        }
    }

    public class DiscontinueMedicationCommandHandler : IRequestHandler<DiscontinueMedicationCommand, Result>
    {
        private readonly PatientDbContext _context;

        public DiscontinueMedicationCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DiscontinueMedicationCommand request, CancellationToken cancellationToken)
        {
            // Find the medication by ID
            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medication == null)
            {
                return Result.Failure($"Medication with ID {request.Id} not found");
            }

            // Discontinue the medication
            medication.Discontinue(request.DiscontinueDate);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}