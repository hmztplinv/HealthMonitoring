// GetAllMedicationsQuery.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Medications.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Medications.Queries
{
    public class GetAllMedicationsQuery : IRequest<Result<List<MedicationDto>>>
    {
        public Guid? PatientId { get; set; }
        public bool? ActiveOnly { get; set; }
    }

    public class GetAllMedicationsQueryHandler : IRequestHandler<GetAllMedicationsQuery, Result<List<MedicationDto>>>
    {
        private readonly PatientDbContext _context;

        public GetAllMedicationsQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MedicationDto>>> Handle(GetAllMedicationsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Medications
                .Include(m => m.Patient)
                .AsQueryable();

            // Filter by patient if specified
            if (request.PatientId.HasValue)
            {
                query = query.Where(m => m.PatientId == request.PatientId.Value);
            }

            // Filter by active status if specified
            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(m => m.IsActive == request.ActiveOnly.Value);
            }

            var medications = await query
                .AsNoTracking()
                .OrderByDescending(m => m.StartDate)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var medicationDtos = medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                PatientId = m.PatientId,
                PatientName = m.Patient.FullName,
                Name = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Instructions = m.Instructions,
                PrescribedBy = m.PrescribedBy,
                PrescribedByUserId = m.PrescribedByUserId,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();

            return Result<List<MedicationDto>>.Success(medicationDtos);
        }
    }
}