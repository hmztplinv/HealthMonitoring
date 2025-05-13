// ActivatePatientCommand.cs
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
    public class ActivatePatientCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActivatePatientCommandValidator : AbstractValidator<ActivatePatientCommand>
    {
        public ActivatePatientCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Patient ID is required");
        }
    }

    public class ActivatePatientCommandHandler : IRequestHandler<ActivatePatientCommand, Result>
    {
        private readonly PatientDbContext _context;

        public ActivatePatientCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ActivatePatientCommand request, CancellationToken cancellationToken)
        {
            // Find the patient by ID
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                return Result.Failure($"Patient with ID {request.Id} not found");
            }

            // Activate the patient
            patient.Activate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}