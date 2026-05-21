using FluentValidation;
using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Entities.Employees;

public class Employee : BaseEntity<Employee>
{
    public string Name { get; private set; }
    public string Cpf { get; private set; }
    public DateTime? HireDate { get; private set; }
    
    public int CompanyId { get; private set; }
    public Company Company { get; private set; }
    
    public string CurrentPositionDescription => 
        GetLastPosition()?.Position?.Description ?? "No link";
    
    public ValidationResult ValidationResult { get; private set; }
    
    private readonly List<EmployeePosition> _positions = new();
    public IReadOnlyCollection<EmployeePosition> Positions => _positions.AsReadOnly();

    protected Employee(){}
    
    public Employee(string name, string cpf, DateTime? hireDate, int companyId)
    {
        Name = name;
        Cpf = StringUtils.OnlyNumbers(cpf);
        HireDate = hireDate;
        CompanyId = companyId;
    }

    public void Update(string name, string cpf)
    {
        Name = name;
        Cpf = StringUtils.OnlyNumbers(cpf);
    }
    
    public override bool Validation()
    {
        if (!RulesRegistered)
        {
            RuleFor(e => e.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("The name must have at least 3 characters.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

            RuleFor(e => e.Cpf)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("CPF is required.")
                .Length(11).WithMessage("CPF must be exactly 11 characters.")
                .Must(CpfValidator.IsValid).WithMessage("The CPF provided is invalid.");

            RuleFor(e => e.HireDate)
                .Must(d => !d.HasValue || d.Value.Date <= DateTime.UtcNow.Date)
                .WithMessage("Hire date cannot be in the future.")
                .Must(d => !d.HasValue || d.Value > new DateTime(1900, 1, 1))
                .WithMessage("The hire date must be after 01/01/1900.");

            RuleFor(e => e.CompanyId)
                .GreaterThan(0)
                .WithMessage("CompanyId must be greater than zero.");

            MarkRulesAsRegistered();
        }

        ValidationResult = Validate(this);
        return ValidationResult.IsValid;
    }
    
    public EmployeePosition? GetLastPosition()
    {
        return _positions
            .OrderByDescending(e => e.StartDate)
            .FirstOrDefault();
    }
}