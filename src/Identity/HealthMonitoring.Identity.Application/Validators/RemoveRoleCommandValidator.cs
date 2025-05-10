using FluentValidation;
using HealthMonitoring.Identity.Application.Commands;

namespace HealthMonitoring.Identity.Application.Validators
{
    /// <summary>
    /// Validator for RemoveRoleCommand
    /// </summary>
    public class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
    {
        public RemoveRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required");
        }
    }
}