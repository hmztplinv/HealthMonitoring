using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HealthMonitoring.Patient.Infrastructure.Data;
using HealthMonitoring.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthMonitoring.Patient.Features.Patients.Commands
{
    public class UpdatePatientCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
    }

    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hasta ID'si gereklidir");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad gereklidir")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad gereklidir")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir");

            RuleFor(x => x.IdentificationNumber)
                .MaximumLength(20).WithMessage("Kimlik numarası en fazla 20 karakter olabilir");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Doğum tarihi gereklidir")
                .LessThan(DateTime.UtcNow).WithMessage("Doğum tarihi bugünden önce olmalıdır");

            RuleFor(x => x.Gender)
                .MaximumLength(10).WithMessage("Cinsiyet en fazla 10 karakter olabilir");

            RuleFor(x => x.BloodType)
                .MaximumLength(5).WithMessage("Kan grubu en fazla 5 karakter olabilir");

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("Adres en fazla 255 karakter olabilir");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olabilir")
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(x => x.EmergencyContactName)
                .MaximumLength(100).WithMessage("Acil durum kişisi adı en fazla 100 karakter olabilir");

            RuleFor(x => x.EmergencyContactPhone)
                .MaximumLength(20).WithMessage("Acil durum kişisi telefonu en fazla 20 karakter olabilir");
        }
    }

    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result>
    {
        private readonly PatientDbContext _context;

        public UpdatePatientCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            // Hastayı ID'ye göre bul
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (patient == null)
            {
                return Result.Failure($"ID'si {request.Id} olan hasta bulunamadı");
            }

            // IdentificationNumber'ın daha önce kullanılıp kullanılmadığını kontrol et
            if (!string.IsNullOrEmpty(request.IdentificationNumber))
            {
                var existingPatientWithIdNumber = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdentificationNumber == request.IdentificationNumber && p.Id != request.Id, cancellationToken);

                if (existingPatientWithIdNumber != null)
                {
                    return Result.Failure($"{request.IdentificationNumber} kimlik numarası ile zaten başka bir hasta kaydı mevcut");
                }
            }

            // Hasta bilgilerini güncelle
            patient.UpdatePersonalInfo(
                request.FirstName,
                request.LastName,
                request.IdentificationNumber,
                request.DateOfBirth,
                request.Gender,
                request.BloodType,
                request.Address,
                request.PhoneNumber,
                request.Email,
                request.EmergencyContactName,
                request.EmergencyContactPhone);

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}