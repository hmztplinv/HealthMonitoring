using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Hospitals.Commands
{
    public class DeactivateHospitalCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateHospitalCommandValidator : AbstractValidator<DeactivateHospitalCommand>
    {
        public DeactivateHospitalCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hospital ID is required");
        }
    }

    public class DeactivateHospitalCommandHandler : IRequestHandler<DeactivateHospitalCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public DeactivateHospitalCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeactivateHospitalCommand request, CancellationToken cancellationToken)
        {
            // Find the hospital by ID
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hospital == null)
            {
                return Result.Failure($"Hospital with ID {request.Id} not found");
            }

            // Deactivate the hospital
            hospital.Deactivate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}