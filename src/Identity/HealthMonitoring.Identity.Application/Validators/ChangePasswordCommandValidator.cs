using FluentValidation;
using HealthMonitoring.Identity.Application.Commands;

namespace HealthMonitoring.Identity.Application.Validators
{
    /// <summary>
    /// Validator for ChangePasswordCommand
    /// </summary>
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters")
                .MaximumLength(100).WithMessage("New password cannot exceed 100 characters")
                .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("New password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character");
        }
    }
}