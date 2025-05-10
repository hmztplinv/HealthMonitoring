using FluentValidation;
using HealthMonitoring.Identity.Application.Authentication;

namespace HealthMonitoring.Identity.Application.Validators
{
    /// <summary>
    /// Validator for AuthenticateUserCommand
    /// </summary>
    public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AuthenticateUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}