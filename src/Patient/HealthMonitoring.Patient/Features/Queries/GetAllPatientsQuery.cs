// GetAllPatientsQuery.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Patients.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Patients.Queries
{
    public class GetAllPatientsQuery : IRequest<Result<List<PatientDto>>>
    {
        public bool? ActiveOnly { get; set; }
    }

    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, Result<List<PatientDto>>>
    {
        private readonly PatientDbContext _context;

        public GetAllPatientsQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Patients.AsQueryable();

            // Filter by active status if specified
            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(p => p.IsActive == request.ActiveOnly.Value);
            }

            var patients = await query
                .AsNoTracking()
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var patientDtos = patients.Select(p => new PatientDto
            {
                Id = p.Id,
                UserId = p.UserId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                FullName = p.FullName,
                IdentificationNumber = p.IdentificationNumber,
                DateOfBirth = p.DateOfBirth,
                Age = p.Age,
                Gender = p.Gender,
                BloodType = p.BloodType,
                Address = p.Address,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email,
                EmergencyContactName = p.EmergencyContactName,
                EmergencyContactPhone = p.EmergencyContactPhone,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return Result<List<PatientDto>>.Success(patientDtos);
        }
    }
}