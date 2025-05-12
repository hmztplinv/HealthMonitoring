// DeactivateStaffMemberCommand.cs
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
    public class DeactivateStaffMemberCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeactivateStaffMemberCommandValidator : AbstractValidator<DeactivateStaffMemberCommand>
    {
        public DeactivateStaffMemberCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Staff member ID is required");
        }
    }

    public class DeactivateStaffMemberCommandHandler : IRequestHandler<DeactivateStaffMemberCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public DeactivateStaffMemberCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeactivateStaffMemberCommand request, CancellationToken cancellationToken)
        {
            // Find the staff member by ID
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (staffMember == null)
            {
                return Result.Failure($"Staff member with ID {request.Id} not found");
            }

            // Deactivate the staff member
            staffMember.Deactivate();

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}