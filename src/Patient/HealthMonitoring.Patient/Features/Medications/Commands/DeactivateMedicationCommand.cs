// DeactivateMedicationCommand.cs
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
    public class DeactivateMedicationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateMedicationCommandValidator : AbstractValidator<DeactivateMedicationCommand>
    {
        public DeactivateMedicationCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Medication ID is required");
        }
    }

    public class DeactivateMedicationCommandHandler : IRequestHandler<DeactivateMedicationCommand, Result>
    {
        private readonly PatientDbContext _context;

        public DeactivateMedicationCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeactivateMedicationCommand request, CancellationToken cancellationToken)
        {
            // Find the medication by ID
            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medication == null)
            {
                return Result.Failure($"Medication with ID {request.Id} not found");
            }

            // Deactivate the medication
            medication.Deactivate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}