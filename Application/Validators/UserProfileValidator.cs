using FluentValidation;
using UserProfileBackend.Application.Models.User;

namespace UserProfileBackend.Application.Validators;

public class UserProfileValidator : AbstractValidator<UserProfile>
{
    public UserProfileValidator()
    {
        // Include rules from BaseModelValidator
        RuleFor(x => x).SetValidator(new BaseModelValidator());

        RuleFor(up => up.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters.");

        RuleFor(up => up.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .MaximumLength(255).WithMessage("Email cannot be longer than 255 characters.");

        RuleFor(up => up.Bio)
            .MaximumLength(500).WithMessage("Bio cannot be longer than 500 characters.");

        RuleFor(up => up.RoleId)
            .NotEmpty().WithMessage("Role is required.")
            .GreaterThan(0UL).WithMessage("RoleId must be a positive number.");
    }
}
