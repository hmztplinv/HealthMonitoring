// ActivateMedicationCommand.cs
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
    public class ActivateMedicationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActivateMedicationCommandValidator : AbstractValidator<ActivateMedicationCommand>
    {
        public ActivateMedicationCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Medication ID is required");
        }
    }

    public class ActivateMedicationCommandHandler : IRequestHandler<ActivateMedicationCommand, Result>
    {
        private readonly PatientDbContext _context;

        public ActivateMedicationCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ActivateMedicationCommand request, CancellationToken cancellationToken)
        {
            // Find the medication by ID
            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medication == null)
            {
                return Result.Failure($"Medication with ID {request.Id} not found");
            }

            // Activate the medication
            medication.Activate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}