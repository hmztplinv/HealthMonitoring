using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Features.Departments.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Departments.Commands
{
    public class CreateDepartmentCommand : IRequest<Result<Guid>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid HospitalId { get; set; }
    }

    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.HospitalId)
                .NotEmpty().WithMessage("Hospital ID is required");
        }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
    {
        private readonly OrganisationDbContext _context;

        public CreateDepartmentCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Check if hospital exists
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == request.HospitalId, cancellationToken);

            if (hospital == null)
            {
                return Result<Guid>.Failure($"Hospital with ID {request.HospitalId} not found");
            }

            // Check if department with the same name already exists in the hospital
            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.HospitalId == request.HospitalId && d.Name == request.Name, cancellationToken);

            if (existingDepartment != null)
            {
                return Result<Guid>.Failure($"Department with name '{request.Name}' already exists in this hospital");
            }

            // Create a new department
            var department = new Department(
                request.Name,
                request.Description,
                request.HospitalId);

            // Add to database
            await _context.Departments.AddAsync(department, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(department.Id);
        }
    }
}