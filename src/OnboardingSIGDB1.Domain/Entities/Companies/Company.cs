using FluentValidation;
using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Utils;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.Entities.Companies;

public class Company : BaseEntity<Company>
{
    public string Name { get; private set; }
    public string Cnpj { get; private set; }
    public DateTime? FoundationDate { get; private set; }
    
    public ValidationResult ValidationResult { get; private set; }
    
    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();
    
    protected Company() { }
    
    public Company(string name, string cnpj, DateTime? foundationDate)
    {
        Name = name;
        Cnpj = StringUtils.OnlyNumbers(cnpj);
        FoundationDate = foundationDate;
    }

    public void Update(string name, string cnpj, DateTime? foundationDate)
    {
        Name = name;
        Cnpj = StringUtils.OnlyNumbers(cnpj);
        FoundationDate = foundationDate;
    }
    
    public override bool Validation()
    {
        if (!RulesRegistered)
        {
            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("The name must have at least 3 characters.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

            RuleFor(c => c.FoundationDate)
                .Must(d => !d.HasValue || d.Value.Date <= DateTime.UtcNow.Date)
                .WithMessage("The founding date cannot be in the future.")
                .Must(d => !d.HasValue || d.Value > new DateTime(1900, 1, 1))
                .WithMessage("The foundation date must be after 01/01/1900.");

            RuleFor(c => c.Cnpj)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("CNPJ is required.")
                .Length(14).WithMessage("CNPJ must be exactly 14 characters.")
                .Must(CnpjValidator.IsValid).WithMessage("CNPJ is invalid.");

            MarkRulesAsRegistered();
        }

        ValidationResult = Validate(this);
        return ValidationResult.IsValid;
    }
}
