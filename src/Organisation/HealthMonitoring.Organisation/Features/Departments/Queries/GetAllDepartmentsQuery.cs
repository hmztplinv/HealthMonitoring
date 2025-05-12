// GetAllDepartmentsQuery.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Departments.Queries.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Departments.Queries
{
    public class GetAllDepartmentsQuery : IRequest<Result<List<DepartmentDto>>>
    {
        public bool? ActiveOnly { get; set; }
        public Guid? HospitalId { get; set; }
    }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Result<List<DepartmentDto>>>
    {
        private readonly OrganisationDbContext _context;

        public GetAllDepartmentsQueryHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Departments
                .Include(d => d.Hospital)
                .AsQueryable();

            // Filter by active status if specified
            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(d => d.IsActive == request.ActiveOnly.Value);
            }

            // Filter by hospital if specified
            if (request.HospitalId.HasValue)
            {
                query = query.Where(d => d.HospitalId == request.HospitalId.Value);
            }

            var departments = await query
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);

            // Get staff counts by department
            var staffCounts = await _context.StaffMembers
                .GroupBy(s => s.DepartmentId)
                .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Build a dictionary of department ID to staff count
            var staffCountDict = staffCounts.ToDictionary(s => s.DepartmentId, s => s.Count);

            // Map to DTOs
            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HospitalId = d.HospitalId,
                HospitalName = d.Hospital.Name,
                IsActive = d.IsActive,
                StaffCount = staffCountDict.ContainsKey(d.Id) ? staffCountDict[d.Id] : 0,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToList();

            return Result<List<DepartmentDto>>.Success(departmentDtos);
        }
    }
}