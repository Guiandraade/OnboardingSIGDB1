using FluentValidation;

namespace OnboardingSIGDB1.Domain.Dto.Common.Filters.Validators;

public class BaseFilterValidator<T> : AbstractValidator<T>
    where T : BaseFilter
{
    protected BaseFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Page number must not exceed 10000.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}