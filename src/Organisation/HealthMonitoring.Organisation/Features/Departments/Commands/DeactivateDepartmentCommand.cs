// DeactivateDepartmentCommand.cs
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
    public class DeactivateDepartmentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateDepartmentCommandValidator : AbstractValidator<DeactivateDepartmentCommand>
    {
        public DeactivateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required");
        }
    }

    public class DeactivateDepartmentCommandHandler : IRequestHandler<DeactivateDepartmentCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public DeactivateDepartmentCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeactivateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Find the department by ID
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (department == null)
            {
                return Result.Failure($"Department with ID {request.Id} not found");
            }

            // Deactivate the department
            department.Deactivate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}