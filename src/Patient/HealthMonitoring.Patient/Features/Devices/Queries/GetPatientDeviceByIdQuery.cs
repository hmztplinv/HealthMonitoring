// GetPatientDeviceByIdQuery.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Devices.Queries.Models;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Devices.Queries
{
    public class GetPatientDeviceByIdQuery : IRequest<Result<PatientDeviceDto>>
    {
        public Guid Id { get; set; }

        public GetPatientDeviceByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetPatientDeviceByIdQueryHandler : IRequestHandler<GetPatientDeviceByIdQuery, Result<PatientDeviceDto>>
    {
        private readonly PatientDbContext _context;

        public GetPatientDeviceByIdQueryHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PatientDeviceDto>> Handle(GetPatientDeviceByIdQuery request, CancellationToken cancellationToken)
        {
            var patientDevice = await _context.PatientDevices
                .Include(d => d.Patient)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (patientDevice == null)
            {
                return Result<PatientDeviceDto>.Failure($"Patient device with ID {request.Id} not found");
            }

            var patientDeviceDto = new PatientDeviceDto
            {
                Id = patientDevice.Id,
                PatientId = patientDevice.PatientId,
                PatientName = patientDevice.Patient.FullName,
                DeviceId = patientDevice.DeviceId,
                DeviceSerialNumber = patientDevice.DeviceSerialNumber,
                DeviceType = patientDevice.DeviceType,
                DeviceTypeName = patientDevice.DeviceType.ToString(),
                AssignedDate = patientDevice.AssignedDate,
                UnassignedDate = patientDevice.UnassignedDate,
                AssignedBy = patientDevice.AssignedBy,
                AssignedByUserId = patientDevice.AssignedByUserId,
                IsActive = patientDevice.IsActive,
                Notes = patientDevice.Notes,
                CreatedAt = patientDevice.CreatedAt,
                UpdatedAt = patientDevice.UpdatedAt
            };

            return Result<PatientDeviceDto>.Success(patientDeviceDto);
        }
    }
}