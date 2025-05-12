// GetStaffMemberByIdQuery.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Staff.Queries.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Staff.Queries
{
    public class GetStaffMemberByIdQuery : IRequest<Result<StaffMemberDto>>
    {
        public Guid Id { get; set; }

        public GetStaffMemberByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetStaffMemberByIdQueryHandler : IRequestHandler<GetStaffMemberByIdQuery, Result<StaffMemberDto>>
    {
        private readonly OrganisationDbContext _context;

        public GetStaffMemberByIdQueryHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<StaffMemberDto>> Handle(GetStaffMemberByIdQuery request, CancellationToken cancellationToken)
        {
            var staffMember = await _context.StaffMembers
                .Include(s => s.Department)
                .ThenInclude(d => d.Hospital)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (staffMember == null)
            {
                return Result<StaffMemberDto>.Failure($"Staff member with ID {request.Id} not found");
            }

            var staffMemberDto = new StaffMemberDto
            {
                Id = staffMember.Id,
                UserId = staffMember.UserId,
                FirstName = staffMember.FirstName,
                LastName = staffMember.LastName,
                FullName = staffMember.FullName,
                Title = staffMember.Title,
                DepartmentId = staffMember.DepartmentId,
                DepartmentName = staffMember.Department.Name,
                HospitalId = staffMember.Department.HospitalId,
                HospitalName = staffMember.Department.Hospital.Name,
                StaffRole = staffMember.StaffRole,
                RoleName = staffMember.StaffRole.ToString(),
                LicenseNumber = staffMember.LicenseNumber,
                IsActive = staffMember.IsActive,
                CreatedAt = staffMember.CreatedAt,
                UpdatedAt = staffMember.UpdatedAt
            };

            return Result<StaffMemberDto>.Success(staffMemberDto);
        }
    }
}