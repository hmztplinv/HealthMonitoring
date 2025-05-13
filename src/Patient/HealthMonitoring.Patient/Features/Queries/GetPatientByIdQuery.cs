// GetPatientByIdQuery.cs
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
    public class GetPatientByIdQuery : IRequest<Result<PatientDto>>
    {
        public Guid Id { get; set; }

        public GetPatientByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
    {
        private readonly PatientDbContext _context;

        public GetPatientByIdQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                return Result<PatientDto>.Failure($"Patient with ID {request.Id} not found");
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