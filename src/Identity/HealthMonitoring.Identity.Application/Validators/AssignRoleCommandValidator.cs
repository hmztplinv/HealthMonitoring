using FluentValidation;
using HealthMonitoring.Identity.Application.Commands;

namespace HealthMonitoring.Identity.Application.Validators
{
    /// <summary>
    /// Validator for AssignRoleCommand
    /// </summary>
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required");
        }
    }
}