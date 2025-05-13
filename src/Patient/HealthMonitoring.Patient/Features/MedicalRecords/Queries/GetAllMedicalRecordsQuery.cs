// GetAllMedicalRecordsQuery.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.MedicalRecords.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Queries
{
    public class GetAllMedicalRecordsQuery : IRequest<Result<List<MedicalRecordDto>>>
    {
        public Guid? PatientId { get; set; }
    }

    public class GetAllMedicalRecordsQueryHandler : IRequestHandler<GetAllMedicalRecordsQuery, Result<List<MedicalRecordDto>>>
    {
        private readonly PatientDbContext _context;

        public GetAllMedicalRecordsQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MedicalRecordDto>>> Handle(GetAllMedicalRecordsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MedicalRecords
                .Include(m => m.Patient)
                .AsQueryable();

            // Filter by patient if specified
            if (request.PatientId.HasValue)
            {
                query = query.Where(m => m.PatientId == request.PatientId.Value);
            }

            var medicalRecords = await query
                .AsNoTracking()
                .OrderByDescending(m => m.RecordDate)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var medicalRecordDtos = medicalRecords.Select(m => new MedicalRecordDto
            {
                Id = m.Id,
                PatientId = m.PatientId,
                PatientName = m.Patient.FullName,
                RecordDate = m.RecordDate,
                RecordType = m.RecordType,
                Description = m.Description,
                Diagnosis = m.Diagnosis,
                Treatment = m.Treatment,
                Notes = m.Notes,
                RecordedByUserId = m.RecordedByUserId,
                RecordedByName = m.RecordedByName,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();

            return Result<List<MedicalRecordDto>>.Success(medicalRecordDtos);
        }
    }
}