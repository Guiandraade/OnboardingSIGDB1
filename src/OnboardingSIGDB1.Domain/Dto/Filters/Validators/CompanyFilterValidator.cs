using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Dto.filters.Validators;

public class CompanyFilterValidator : BaseFilterValidator<CompanyFilter>
{
    public CompanyFilterValidator()
    {
        RuleFor(x => x.FoundedUntil)
            .GreaterThanOrEqualTo(x => x.FoundedIn) // O FluentValidation lida com o .Value internamente se comparar dois Nullables
            .When(x => x.FoundedIn.HasValue && x.FoundedUntil.HasValue)
            .WithMessage("The 'Founded Until' date must be greater than or equal to 'Founded In'.");

        RuleFor(x => x.FoundedIn)
            .GreaterThan(new DateTime(1753, 1, 1))
            .When(x => x.FoundedIn.HasValue)
            .WithMessage("The 'Founded In' date must be after 01/01/1753.");

        RuleFor(x => x.FoundedUntil)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.FoundedUntil.HasValue)
            .WithMessage("The 'Founded Until' date cannot be in the future.");
        
    }
}