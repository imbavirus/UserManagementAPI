using FluentValidation;
using UserManagementAPI.Application.Models.User;

namespace UserManagementAPI.Application.Validators;

public class RoleValidator : AbstractValidator<Role>
{
    public RoleValidator()
    {
        // Include rules from BaseModelValidator
        RuleFor(x => x).SetValidator(new BaseModelValidator());

        RuleFor(role => role.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(50).WithMessage("Role name cannot be longer than 50 characters.");
    }
}