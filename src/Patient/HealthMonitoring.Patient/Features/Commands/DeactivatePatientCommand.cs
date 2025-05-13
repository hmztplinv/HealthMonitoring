// DeactivatePatientCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Patients.Commands
{
    public class DeactivatePatientCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeactivatePatientCommandValidator : AbstractValidator<DeactivatePatientCommand>
    {
        public DeactivatePatientCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Patient ID is required");
        }
    }

    public class DeactivatePatientCommandHandler : IRequestHandler<DeactivatePatientCommand, Result>
    {
        private readonly PatientDbContext _context;

        public DeactivatePatientCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeactivatePatientCommand request, CancellationToken cancellationToken)
        {
            // Find the patient by ID
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                return Result.Failure($"Patient with ID {request.Id} not found");
            }

            // Deactivate the patient
            patient.Deactivate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}