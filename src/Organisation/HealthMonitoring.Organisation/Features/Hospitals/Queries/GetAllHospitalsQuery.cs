using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Hospitals.Queries.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Hospitals.Queries
{
    public class GetAllHospitalsQuery : IRequest<Result<List<HospitalDto>>>
    {
        public bool? ActiveOnly { get; set; }
    }

    public class GetAllHospitalsQueryHandler : IRequestHandler<GetAllHospitalsQuery, Result<List<HospitalDto>>>
    {
        private readonly OrganisationDbContext _context;

        public GetAllHospitalsQueryHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<HospitalDto>>> Handle(GetAllHospitalsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Hospitals.AsQueryable();

            // Filter by active status if specified
            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(h => h.IsActive == request.ActiveOnly.Value);
            }

            var hospitals = await query
                .AsNoTracking()
                .OrderBy(h => h.Name)
                .ToListAsync(cancellationToken);

            // Get all departments grouped by hospital
            var departmentCounts = await _context.Departments
                .GroupBy(d => d.HospitalId)
                .Select(g => new { HospitalId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Build a dictionary of hospital ID to department count
            var departmentCountDict = departmentCounts.ToDictionary(d => d.HospitalId, d => d.Count);

            // Get staff counts by hospital
            var staffCounts = await _context.StaffMembers
                .Join(_context.Departments,
                    s => s.DepartmentId,
                    d => d.Id,
                    (s, d) => new { HospitalId = d.HospitalId })
                .GroupBy(x => x.HospitalId)
                .Select(g => new { HospitalId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Build a dictionary of hospital ID to staff count
            var staffCountDict = staffCounts.ToDictionary(s => s.HospitalId, s => s.Count);

            // Map to DTOs
            var hospitalDtos = hospitals.Select(h => new HospitalDto
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                PhoneNumber = h.PhoneNumber,
                Email = h.Email,
                IsActive = h.IsActive,
                DepartmentCount = departmentCountDict.ContainsKey(h.Id) ? departmentCountDict[h.Id] : 0,
                StaffCount = staffCountDict.ContainsKey(h.Id) ? staffCountDict[h.Id] : 0,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            }).ToList();

            return Result<List<HospitalDto>>.Success(hospitalDtos);
        }
    }
}