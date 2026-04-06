using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Dto.filters.Validators;

public class PositionFilterValidator : BaseFilterValidator<PositionFilter>
{
    public PositionFilterValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(100)
            .WithMessage("The description filter cannot exceed 100 characters.");
    }
}