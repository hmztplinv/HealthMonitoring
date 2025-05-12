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
    public class UpdateHospitalCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class UpdateHospitalCommandValidator : AbstractValidator<UpdateHospitalCommand>
    {
        public UpdateHospitalCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hospital ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Hospital name is required")
                .MaximumLength(100).WithMessage("Hospital name cannot exceed 100 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(255).WithMessage("Address cannot exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email is not valid");
        }
    }

    public class UpdateHospitalCommandHandler : IRequestHandler<UpdateHospitalCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public UpdateHospitalCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateHospitalCommand request, CancellationToken cancellationToken)
        {
            // Find the hospital by ID
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hospital == null)
            {
                return Result.Failure($"Hospital with ID {request.Id} not found");
            }

            // Check if another hospital with the same name already exists
            var existingHospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Name == request.Name && h.Id != request.Id, cancellationToken);

            if (existingHospital != null)
            {
                return Result.Failure($"Another hospital with name '{request.Name}' already exists");
            }

            // Update hospital details
            hospital.UpdateDetails(
                request.Name,
                request.Address,
                request.PhoneNumber,
                request.Email);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}