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
    public class DeleteStaffMemberCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteStaffMemberCommandValidator : AbstractValidator<DeleteStaffMemberCommand>
    {
        public DeleteStaffMemberCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Staff member ID is required");
        }
    }

    public class DeleteStaffMemberCommandHandler : IRequestHandler<DeleteStaffMemberCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public DeleteStaffMemberCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteStaffMemberCommand request, CancellationToken cancellationToken)
        {
            // Find the staff member by ID
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (staffMember == null)
            {
                return Result.Failure($"Staff member with ID {request.Id} not found");
            }

            // Remove the staff member
            _context.StaffMembers.Remove(staffMember);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}