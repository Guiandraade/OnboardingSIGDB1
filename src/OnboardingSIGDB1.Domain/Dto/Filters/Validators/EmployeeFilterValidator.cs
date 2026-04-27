using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Dto.Filters.Validators;

public class EmployeeFilterValidator : BaseFilterValidator<EmployeeFilter>
{
    public EmployeeFilterValidator()
    {
        RuleFor(x => x.HiredUntil)
            .GreaterThanOrEqualTo(x => x.HiredFrom)
            .When(x => x.HiredFrom.HasValue && x.HiredUntil.HasValue)
            .WithMessage("The 'Hired Until' date must be greater than or equal to 'Hired From'.");

        RuleFor(x => x.HiredFrom)
            .GreaterThan(new DateTime(1753, 1, 1))
            .When(x => x.HiredFrom.HasValue)
            .WithMessage("The start date is invalid.");

        RuleFor(x => x.HiredUntil)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.HiredUntil.HasValue)
            .WithMessage("The end date cannot be in the future.");

        RuleFor(x => x.Cpf)
            .MaximumLength(14).WithMessage("The CPF provided for filtering is too long.")
            // Opcional: Garante que se houver CPF, ele tenha pelo menos o tamanho de números
            .MinimumLength(11).When(x => !string.IsNullOrWhiteSpace(x.Cpf))
            .WithMessage("The CPF provided is too short.");
        
    }
}