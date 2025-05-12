// UpdateStaffMemberCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Staff.Commands
{
    public class UpdateStaffMemberCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public UserRole StaffRole { get; set; }
        public string LicenseNumber { get; set; }
    }

    public class UpdateStaffMemberCommandValidator : AbstractValidator<UpdateStaffMemberCommand>
    {
        public UpdateStaffMemberCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Staff member ID is required");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage("Title cannot exceed 50 characters");

            RuleFor(x => x.StaffRole)
                .IsInEnum().WithMessage("Invalid staff role");

            RuleFor(x => x.LicenseNumber)
                .MaximumLength(50).WithMessage("License number cannot exceed 50 characters");
        }
    }

    public class UpdateStaffMemberCommandHandler : IRequestHandler<UpdateStaffMemberCommand, Result>
    {
        private readonly OrganisationDbContext _context;

        public UpdateStaffMemberCommandHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateStaffMemberCommand request, CancellationToken cancellationToken)
        {
            // Find the staff member by ID
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (staffMember == null)
            {
                return Result.Failure($"Staff member with ID {request.Id} not found");
            }

            // Check if a staff member with the same license number already exists (if license number is provided)
            if (!string.IsNullOrEmpty(request.LicenseNumber) && request.LicenseNumber != staffMember.LicenseNumber)
            {
                var existingLicense = await _context.StaffMembers
                    .FirstOrDefaultAsync(s => s.LicenseNumber == request.LicenseNumber, cancellationToken);

                if (existingLicense != null)
                {
                    return Result.Failure($"A staff member with license number '{request.LicenseNumber}' already exists");
                }
            }

            // Update staff member details
            staffMember.UpdateDetails(
                request.FirstName,
                request.LastName,
                request.Title,
                request.StaffRole,
                request.LicenseNumber);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}