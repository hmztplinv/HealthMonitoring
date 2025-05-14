// CreateStaffMemberCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Organisation.Features.Staff.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.EventModels;
using HealthMonitoring.SharedKernel.Messaging;
using HealthMonitoring.SharedKernel.Results;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Staff.Commands
{
    public class CreateStaffMemberCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public Guid DepartmentId { get; set; }
        public UserRole StaffRole { get; set; }
        public string LicenseNumber { get; set; }
    }

    public class CreateStaffMemberCommandValidator : AbstractValidator<CreateStaffMemberCommand>
    {
        public CreateStaffMemberCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage("Title cannot exceed 50 characters");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required");

            RuleFor(x => x.StaffRole)
                .IsInEnum().WithMessage("Invalid staff role");

            RuleFor(x => x.LicenseNumber)
                .MaximumLength(50).WithMessage("License number cannot exceed 50 characters");
        }
    }

    public class CreateStaffMemberCommandHandler : IRequestHandler<CreateStaffMemberCommand, Result<Guid>>
    {
        private readonly OrganisationDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateStaffMemberCommandHandler> _logger;

        public CreateStaffMemberCommandHandler(
            OrganisationDbContext context,
            IPublishEndpoint publishEndpoint,
            ILogger<CreateStaffMemberCommandHandler> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateStaffMemberCommand request, CancellationToken cancellationToken)
        {
            // Check if department exists
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

            if (department == null)
            {
                return Result<Guid>.Failure($"Department with ID {request.DepartmentId} not found");
            }

            // Check if a staff member with the same user ID already exists
            var existingStaffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

            if (existingStaffMember != null)
            {
                return Result<Guid>.Failure($"A staff member with User ID {request.UserId} already exists");
            }

            // Check if a staff member with the same license number already exists (if license number is provided)
            if (!string.IsNullOrEmpty(request.LicenseNumber))
            {
                var existingLicense = await _context.StaffMembers
                    .FirstOrDefaultAsync(s => s.LicenseNumber == request.LicenseNumber, cancellationToken);

                if (existingLicense != null)
                {
                    return Result<Guid>.Failure($"A staff member with license number '{request.LicenseNumber}' already exists");
                }
            }

            // Create a new staff member
            var staffMember = new StaffMember(
                request.UserId,
                request.FirstName,
                request.LastName,
                request.Title,
                request.DepartmentId,
                request.StaffRole,
                request.LicenseNumber);

            // Event yayınlama
            var staffCreatedEvent = new StaffCreatedEvent(
                staffMember.Id,
                staffMember.UserId,
                staffMember.FirstName,
                staffMember.LastName,
                staffMember.DepartmentId,
                staffMember.StaffRole);
                
            await _publishEndpoint.PublishEventWithLogging(staffCreatedEvent, _logger);

            // Add to database
            await _context.StaffMembers.AddAsync(staffMember, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(staffMember.Id);
        }
    }
}