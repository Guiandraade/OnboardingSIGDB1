using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Dto.filters.Validators;

public class CompanyFilterValidator : BaseFilterValidator<CompanyFilter>
{
    public CompanyFilterValidator()
    {
        RuleFor(x => x.FoundedUntil)
            .GreaterThanOrEqualTo(x => x.FoundedIn.Value)
            .When(x => x.FoundedIn.HasValue && x.FoundedUntil.HasValue)
            .WithMessage("The 'Founded Until' date must be greater than or equal to 'Founded In'.");

        RuleFor(x => x.FoundedIn)
            .GreaterThan(new DateTime(1753, 1, 1))
            .When(x => x.FoundedIn.HasValue)
            .WithMessage("Invalid start date.");
        
    }
}