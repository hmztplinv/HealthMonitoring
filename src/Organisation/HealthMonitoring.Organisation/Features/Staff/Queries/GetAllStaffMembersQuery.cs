// GetAllStaffMembersQuery.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Staff.Queries.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Staff.Queries
{
    public class GetAllStaffMembersQuery : IRequest<Result<List<StaffMemberDto>>>
    {
        public bool? ActiveOnly { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? HospitalId { get; set; }
        public UserRole? StaffRole { get; set; }
    }

    public class GetAllStaffMembersQueryHandler : IRequestHandler<GetAllStaffMembersQuery, Result<List<StaffMemberDto>>>
    {
        private readonly OrganisationDbContext _context;

        public GetAllStaffMembersQueryHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<StaffMemberDto>>> Handle(GetAllStaffMembersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.StaffMembers
                .Include(s => s.Department)
                .ThenInclude(d => d.Hospital)
                .AsQueryable();

            // Filter by active status if specified
            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(s => s.IsActive == request.ActiveOnly.Value);
            }

            // Filter by department if specified
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == request.DepartmentId.Value);
            }

            // Filter by hospital if specified
            if (request.HospitalId.HasValue)
            {
                query = query.Where(s => s.Department.HospitalId == request.HospitalId.Value);
            }

            // Filter by staff role if specified
            if (request.StaffRole.HasValue)
            {
                query = query.Where(s => s.StaffRole == request.StaffRole.Value);
            }

            var staffMembers = await query
                .AsNoTracking()
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var staffMemberDtos = staffMembers.Select(s => new StaffMemberDto
            {
                Id = s.Id,
                UserId = s.UserId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                FullName = s.FullName,
                Title = s.Title,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department.Name,
                HospitalId = s.Department.HospitalId,
                HospitalName = s.Department.Hospital.Name,
                StaffRole = s.StaffRole,
                RoleName = s.StaffRole.ToString(),
                LicenseNumber = s.LicenseNumber,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Result<List<StaffMemberDto>>.Success(staffMemberDtos);
        }
    }
}