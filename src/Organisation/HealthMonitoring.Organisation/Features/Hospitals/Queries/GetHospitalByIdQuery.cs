
using HealthMonitoring.Organisation.Features.Hospitals.Queries.Models;
using HealthMonitoring.Organisation.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Organisation.Features.Hospitals.Queries
{
    public class GetHospitalByIdQuery : IRequest<Result<HospitalDto>>
    {
        public Guid Id { get; set; }

        public GetHospitalByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetHospitalByIdQueryHandler : IRequestHandler<GetHospitalByIdQuery, Result<HospitalDto>>
    {
        private readonly OrganisationDbContext _context;

        public GetHospitalByIdQueryHandler(OrganisationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<HospitalDto>> Handle(GetHospitalByIdQuery request, CancellationToken cancellationToken)
        {
            var hospital = await _context.Hospitals
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hospital == null)
            {
                return Result<HospitalDto>.Failure($"Hospital with ID {request.Id} not found");
            }

            // Get department count for the hospital
            var departmentCount = await _context.Departments
                .CountAsync(d => d.HospitalId == hospital.Id, cancellationToken);

            // Get staff count for the hospital
            var staffCount = await _context.StaffMembers
                .Where(s => s.Department.HospitalId == hospital.Id)
                .CountAsync(cancellationToken);

            var hospitalDto = new HospitalDto
            {
                Id = hospital.Id,
                Name = hospital.Name,
                Address = hospital.Address,
                PhoneNumber = hospital.PhoneNumber,
                Email = hospital.Email,
                IsActive = hospital.IsActive,
                DepartmentCount = departmentCount,
                StaffCount = staffCount,
                CreatedAt = hospital.CreatedAt,
                UpdatedAt = hospital.UpdatedAt
            };

            return Result<HospitalDto>.Success(hospitalDto);
        }
    }
}