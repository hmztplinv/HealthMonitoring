// TransferStaffMemberCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Staff.Commands
{
    public class TransferStaffMemberCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
    }

    public class TransferStaffMemberCommandValidator : AbstractValidator<TransferStaffMemberCommand>
    {
        public TransferStaffMemberCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Staff member ID is required");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required");
        }
    }

    public class TransferStaffMemberCommandHandler : IRequestHandler<TransferStaffMemberCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public TransferStaffMemberCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(TransferStaffMemberCommand request, CancellationToken cancellationToken)
        {
            // Find the staff member by ID
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (staffMember == null)
            {
                return Result.Failure($"Staff member with ID {request.Id} not found");
            }

            // Check if department exists
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

            if (department == null)
            {
                return Result.Failure($"Department with ID {request.DepartmentId} not found");
            }

            // Transfer staff member to the new department
            staffMember.TransferToDepartment(request.DepartmentId);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}