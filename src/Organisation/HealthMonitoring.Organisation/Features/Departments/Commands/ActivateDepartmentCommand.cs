// ActivateDepartmentCommand.cs
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
    public class ActivateDepartmentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActivateDepartmentCommandValidator : AbstractValidator<ActivateDepartmentCommand>
    {
        public ActivateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required");
        }
    }

    public class ActivateDepartmentCommandHandler : IRequestHandler<ActivateDepartmentCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public ActivateDepartmentCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ActivateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Find the department by ID
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (department == null)
            {
                return Result.Failure($"Department with ID {request.Id} not found");
            }

            // Activate the department
            department.Activate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}