// GetMedicationByIdQuery.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Medications.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Medications.Queries
{
    public class GetMedicationByIdQuery : IRequest<Result<MedicationDto>>
    {
        public Guid Id { get; set; }

        public GetMedicationByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetMedicationByIdQueryHandler : IRequestHandler<GetMedicationByIdQuery, Result<MedicationDto>>
    {
        private readonly PatientDbContext _context;

        public GetMedicationByIdQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MedicationDto>> Handle(GetMedicationByIdQuery request, CancellationToken cancellationToken)
        {
            var medication = await _context.Medications
                .Include(m => m.Patient)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medication == null)
            {
                return Result<MedicationDto>.Failure($"Medication with ID {request.Id} not found");
            }

            var medicationDto = new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                PatientName = medication.Patient.FullName,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                Instructions = medication.Instructions,
                PrescribedBy = medication.PrescribedBy,
                PrescribedByUserId = medication.PrescribedByUserId,
                IsActive = medication.IsActive,
                CreatedAt = medication.CreatedAt,
                UpdatedAt = medication.UpdatedAt
            };

            return Result<MedicationDto>.Success(medicationDto);
        }
    }
}