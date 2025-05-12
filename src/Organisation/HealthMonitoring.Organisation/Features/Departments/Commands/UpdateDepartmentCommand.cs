// UpdateDepartmentCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Departments.Commands
{
    public class UpdateDepartmentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public UpdateDepartmentCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Find the department by ID
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (department == null)
            {
                return Result.Failure($"Department with ID {request.Id} not found");
            }

            // Check if another department with the same name already exists in the same hospital
            var existingDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name == request.Name && 
                                        d.HospitalId == department.HospitalId && 
                                        d.Id != request.Id, 
                                      cancellationToken);

            if (existingDepartment != null)
            {
                return Result.Failure($"Another department with name '{request.Name}' already exists in this hospital");
            }

            // Update department details
            department.UpdateDetails(
                request.Name,
                request.Description);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}