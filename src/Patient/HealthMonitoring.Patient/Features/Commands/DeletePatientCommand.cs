using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.Patient.Features.Patients.Commands
{
    public class DeletePatientCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }

    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result<Guid>>
    {
        private readonly PatientDbContext _dbContext;
        private readonly ILogger<DeletePatientCommandHandler> _logger;

        public DeletePatientCommandHandler(PatientDbContext dbContext, ILogger<DeletePatientCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                _logger.LogWarning("Attempt to delete non-existent patient with ID: {PatientId}", request.Id);
                return Result<Guid>.Failure("Patient not found");
            }

            try
            {
                _dbContext.Patients.Remove(patient);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Patient deleted successfully: {PatientId}", patient.Id);
                return Result<Guid>.Success(patient.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting patient: {PatientId}", request.Id);
                return Result<Guid>.Failure($"Failed to delete patient: {ex.Message}");
            }
        }
    }
} 