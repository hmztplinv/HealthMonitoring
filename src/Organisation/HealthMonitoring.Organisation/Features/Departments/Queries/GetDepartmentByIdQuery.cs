// GetDepartmentByIdQuery.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Departments.Queries.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Departments.Queries
{
    public class GetDepartmentByIdQuery : IRequest<Result<DepartmentDto>>
    {
        public Guid Id { get; set; }

        public GetDepartmentByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
    {
        private readonly OrganisationDbContext _context;

        public GetDepartmentByIdQueryHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var department = await _context.Departments
                .Include(d => d.Hospital)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (department == null)
            {
                return Result<DepartmentDto>.Failure($"Department with ID {request.Id} not found");
            }

            // Get staff count for the department
            var staffCount = await _context.StaffMembers
                .CountAsync(s => s.DepartmentId == department.Id, cancellationToken);

            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                HospitalId = department.HospitalId,
                HospitalName = department.Hospital.Name,
                IsActive = department.IsActive,
                StaffCount = staffCount,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt
            };

            return Result<DepartmentDto>.Success(departmentDto);
        }
    }
}