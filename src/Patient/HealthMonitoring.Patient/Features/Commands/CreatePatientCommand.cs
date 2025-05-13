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
    public class CreatePatientCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
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

    public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
    {
        public CreatePatientCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı ID'si gereklidir");

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

    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Guid>>
    {
        private readonly PatientDbContext _context;

        public CreatePatientCommandHandler(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            // UserId'nin daha önce kullanılıp kullanılmadığını kontrol et
            var existingPatientWithUserId = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (existingPatientWithUserId != null)
            {
                return Result<Guid>.Failure($"{request.UserId} Kullanıcı ID'si ile zaten bir hasta kaydı mevcut");
            }

            // IdentificationNumber'ın daha önce kullanılıp kullanılmadığını kontrol et
            if (!string.IsNullOrEmpty(request.IdentificationNumber))
            {
                var existingPatientWithIdNumber = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdentificationNumber == request.IdentificationNumber, cancellationToken);

                if (existingPatientWithIdNumber != null)
                {
                    return Result<Guid>.Failure($"{request.IdentificationNumber} kimlik numarası ile zaten bir hasta kaydı mevcut");
                }
            }

            // Yeni hasta oluştur
            var patient = new Models.Patient(
                request.UserId,
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

            // Veritabanına ekle
            await _context.Patients.AddAsync(patient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(patient.Id);
        }
    }
}