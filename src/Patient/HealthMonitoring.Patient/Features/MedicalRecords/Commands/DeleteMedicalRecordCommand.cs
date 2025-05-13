// DeleteMedicalRecordCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.MedicalRecords.Commands
{
    public class DeleteMedicalRecordCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteMedicalRecordCommandValidator : AbstractValidator<DeleteMedicalRecordCommand>
    {
        public DeleteMedicalRecordCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Medical record ID is required");
        }
    }

    public class DeleteMedicalRecordCommandHandler : IRequestHandler<DeleteMedicalRecordCommand, Result>
    {
        private readonly PatientDbContext _context;

        public DeleteMedicalRecordCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            // Find the medical record by ID
            var medicalRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medicalRecord == null)
            {
                return Result.Failure($"Medical record with ID {request.Id} not found");
            }

            // Remove the medical record
            _context.MedicalRecords.Remove(medicalRecord);

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}