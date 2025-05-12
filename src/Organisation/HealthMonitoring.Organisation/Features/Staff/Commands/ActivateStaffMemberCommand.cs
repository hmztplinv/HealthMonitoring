// ActivateStaffMemberCommand.cs
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
    public class ActivateStaffMemberCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActivateStaffMemberCommandValidator : AbstractValidator<ActivateStaffMemberCommand>
    {
        public ActivateStaffMemberCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Staff member ID is required");
        }
    }

    public class ActivateStaffMemberCommandHandler : IRequestHandler<ActivateStaffMemberCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public ActivateStaffMemberCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ActivateStaffMemberCommand request, CancellationToken cancellationToken)
        {
            // Find the staff member by ID
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (staffMember == null)
            {
                return Result.Failure($"Staff member with ID {request.Id} not found");
            }

            // Activate the staff member
            staffMember.Activate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}