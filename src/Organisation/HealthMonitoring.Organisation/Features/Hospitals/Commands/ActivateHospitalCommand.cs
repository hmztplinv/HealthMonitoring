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
    public class ActivateHospitalCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActivateHospitalCommandValidator : AbstractValidator<ActivateHospitalCommand>
    {
        public ActivateHospitalCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hospital ID is required");
        }
    }

    public class ActivateHospitalCommandHandler : IRequestHandler<ActivateHospitalCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public ActivateHospitalCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ActivateHospitalCommand request, CancellationToken cancellationToken)
        {
            // Find the hospital by ID
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hospital == null)
            {
                return Result.Failure($"Hospital with ID {request.Id} not found");
            }

            // Activate the hospital
            hospital.Activate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}