using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Dto.Filters.Validators;

public class PositionFilterValidator : BaseFilterValidator<PositionFilter>
{
    public PositionFilterValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(100)
            .WithMessage("The description filter cannot exceed 100 characters.")
            .Must(d => string.IsNullOrWhiteSpace(d) || d.Trim().Length >= 2)
            .WithMessage("The description filter must have at least 2 characters if provided.");
    }
}