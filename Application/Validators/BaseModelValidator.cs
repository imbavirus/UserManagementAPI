using FluentValidation;
using UserManagementAPI.Application.Models;

namespace UserManagementAPI.Application.Validators;

public class BaseModelValidator : AbstractValidator<BaseModel>
{
    public BaseModelValidator()
    {
        RuleFor(bm => bm.Id)
            .GreaterThanOrEqualTo(0UL).WithMessage("Id must be greater than or equal to 0.");

        RuleFor(bm => bm.Guid)
            .NotEmpty().WithMessage("Guid cannot be empty.")
            .NotEqual(Guid.Empty).WithMessage("Guid must be a valid GUID.");
        
        // no validation for CreatedOn and UpdatedOn as they are managed by the api/database
    }
}
