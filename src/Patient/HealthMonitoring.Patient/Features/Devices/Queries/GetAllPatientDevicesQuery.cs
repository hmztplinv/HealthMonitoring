// GetAllPatientDevicesQuery.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Devices.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Devices.Queries
{
    public class GetAllPatientDevicesQuery : IRequest<Result<List<PatientDeviceDto>>>
    {
        public Guid? PatientId { get; set; }
        public bool? ActiveOnly { get; set; }
    }

    public class GetAllPatientDevicesQueryHandler : IRequestHandler<GetAllPatientDevicesQuery, Result<List<PatientDeviceDto>>>
    {
        private readonly PatientDbContext _context;

        public GetAllPatientDevicesQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PatientDeviceDto>>> Handle(GetAllPatientDevicesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PatientDevices
                .Include(d => d.Patient)
                .AsQueryable();

            // Filter by patient if specified
            if (request.PatientId.HasValue)
            {
                query = query.Where(d => d.PatientId == request.PatientId.Value);
            }

            // Filter by active status if specified
            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(d => d.IsActive == request.ActiveOnly.Value);
            }

            var patientDevices = await query
                .AsNoTracking()
                .OrderByDescending(d => d.AssignedDate)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var patientDeviceDtos = patientDevices.Select(d => new PatientDeviceDto
            {
                Id = d.Id,
                PatientId = d.PatientId,
                PatientName = d.Patient.FullName,
                DeviceId = d.DeviceId,
                DeviceSerialNumber = d.DeviceSerialNumber,
                DeviceType = d.DeviceType,
                DeviceTypeName = d.DeviceType.ToString(),
                AssignedDate = d.AssignedDate,
                UnassignedDate = d.UnassignedDate,
                AssignedBy = d.AssignedBy,
                AssignedByUserId = d.AssignedByUserId,
                IsActive = d.IsActive,
                Notes = d.Notes,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToList();

            return Result<List<PatientDeviceDto>>.Success(patientDeviceDtos);
        }
    }
}