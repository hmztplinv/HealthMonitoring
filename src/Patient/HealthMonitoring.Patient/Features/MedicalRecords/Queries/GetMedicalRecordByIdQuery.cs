// GetMedicalRecordByIdQuery.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.MedicalRecords.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Queries
{
    public class GetMedicalRecordByIdQuery : IRequest<Result<MedicalRecordDto>>
    {
        public Guid Id { get; set; }

        public GetMedicalRecordByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetMedicalRecordByIdQueryHandler : IRequestHandler<GetMedicalRecordByIdQuery, Result<MedicalRecordDto>>
    {
        private readonly PatientDbContext _context;

        public GetMedicalRecordByIdQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MedicalRecordDto>> Handle(GetMedicalRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Patient)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medicalRecord == null)
            {
                return Result<MedicalRecordDto>.Failure($"Medical record with ID {request.Id} not found");
            }

            var medicalRecordDto = new MedicalRecordDto
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                PatientName = medicalRecord.Patient.FullName,
                RecordDate = medicalRecord.RecordDate,
                RecordType = medicalRecord.RecordType,
                Description = medicalRecord.Description,
                Diagnosis = medicalRecord.Diagnosis,
                Treatment = medicalRecord.Treatment,
                Notes = medicalRecord.Notes,
                RecordedByUserId = medicalRecord.RecordedByUserId,
                RecordedByName = medicalRecord.RecordedByName,
                CreatedAt = medicalRecord.CreatedAt,
                UpdatedAt = medicalRecord.UpdatedAt
            };

            return Result<MedicalRecordDto>.Success(medicalRecordDto);
        }
    }
}