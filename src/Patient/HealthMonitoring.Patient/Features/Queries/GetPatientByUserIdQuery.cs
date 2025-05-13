// GetPatientByUserIdQuery.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Patients.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Patients.Queries
{
    public class GetPatientByUserIdQuery : IRequest<Result<PatientDto>>
    {
        public Guid UserId { get; set; }

        public GetPatientByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetPatientByUserIdQueryHandler : IRequestHandler<GetPatientByUserIdQuery, Result<PatientDto>>
    {
        private readonly PatientDbContext _context;

        public GetPatientByUserIdQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PatientDto>> Handle(GetPatientByUserIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (patient == null)
            {
                return Result<PatientDto>.Failure($"Patient with User ID {request.UserId} not found");
            }

            var patientDto = new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                FullName = patient.FullName,
                IdentificationNumber = patient.IdentificationNumber,
                DateOfBirth = patient.DateOfBirth,
                Age = patient.Age,
                Gender = patient.Gender,
                BloodType = patient.BloodType,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                IsActive = patient.IsActive,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt
            };

            return Result<PatientDto>.Success(patientDto);
        }
    }
}