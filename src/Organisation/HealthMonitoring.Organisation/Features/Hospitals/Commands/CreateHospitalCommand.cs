using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Features.Hospitals.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Hospitals.Commands
{
    public class CreateHospitalCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class CreateHospitalCommandValidator : AbstractValidator<CreateHospitalCommand>
    {
        public CreateHospitalCommandValidator()
        {
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

    public class CreateHospitalCommandHandler : IRequestHandler<CreateHospitalCommand, Result<Guid>>
    {
        private readonly OrganisationDbContext _context;

        public CreateHospitalCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateHospitalCommand request, CancellationToken cancellationToken)
        {
            // Check if hospital with the same name already exists
            var existingHospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Name == request.Name, cancellationToken);

            if (existingHospital != null)
            {
                return Result<Guid>.Failure($"Hospital with name '{request.Name}' already exists");
            }

            // Create a new hospital
            var hospital = new Hospital(
                request.Name,
                request.Address,
                request.PhoneNumber,
                request.Email);

            // Add to database
            await _context.Hospitals.AddAsync(hospital, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(hospital.Id);
        }
    }
}