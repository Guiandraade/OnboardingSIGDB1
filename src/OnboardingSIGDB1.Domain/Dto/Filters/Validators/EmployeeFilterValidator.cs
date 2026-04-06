using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Dto.filters.Validators;

public class EmployeeFilterValidator : BaseFilterValidator<EmployeeFilter>
{
    public EmployeeFilterValidator()
    {
        RuleFor(x => x.HiredUntil)
            .Must((filter, hiredUntil) => hiredUntil >= filter.HiredFrom)
            .When(x => x.HiredFrom.HasValue && x.HiredUntil.HasValue)
            .WithMessage("The 'Hired Until' date must be greater than or equal to 'Hired From'.");

        RuleFor(x => x.HiredFrom)
            .GreaterThan(new DateTime(1753, 1, 1))
            .When(x => x.HiredFrom.HasValue)
            .WithMessage("The start date is invalid.");

        RuleFor(x => x.Cpf)
            .MaximumLength(14)
            .WithMessage("The CPF provided for filtering is too long.");
        
    }
}