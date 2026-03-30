using FluentValidation;
using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Entities.Employees;

public class Employee : BaseEntity<Employee>
{
    public string Name { get; private set; }
    public string Cpf { get; private set; }
    public DateTime? HireDate { get; private set; }
    
    public int CompanyId { get; private set; }
    public Company Company { get; private set; }
    
    public ValidationResult ValidationResult { get; private set; }
    
    private readonly List<EmployeePosition> _positions = new();
    public IReadOnlyCollection<EmployeePosition> Positions => _positions.AsReadOnly();

    protected Employee(){}
    
    public Employee(string name, string cpf, DateTime? hireDate)
    {
        Name = name;
        Cpf = StringUtils.OnlyNumbers(cpf);
        HireDate = hireDate;
    }

    public void SetCompany(int companyId)
    {
        if (CompanyId > 0)
        {
            AddNotification("CompanyId", "The link to the company cannot be changed.");
            return;
        }

        if (companyId <= 0)
        {
            AddNotification("CompanyId", "The company ID is required.");
            return;
        }
        
        CompanyId = companyId;
    }

    public override bool Validation()
    {
        ClearNotifications();
        
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
        
        RuleFor(e => e.Cpf)
            .NotEmpty().WithMessage("CPF is required.")
            .Length(11).WithMessage("CPF must not exceed 11 characters.")
            .Must(CpfValidator.IsValid).WithMessage("The CPF provided is invalid.");
        
        RuleFor(e => e.HireDate)
            .Must(d => !d.HasValue || d.Value > DateTime.MinValue)
            .WithMessage("Invalid hiring data.");
        
        ValidationResult = Validate(this);

        if (!ValidationResult.IsValid)
        {
            foreach (var error in ValidationResult.Errors)
            {
                AddNotification(error.PropertyName, error.ErrorMessage);
            }
        }

        return IsValid;
    }
    
    public void AddPosition(Position position, DateTime startDate)
    {
        if(CompanyId == 0)
            AddNotification("Company", "Employee must be linked to a company before assigning a position.");
        
        if(position is null)
            AddNotification("Position", "Position is required.");
        
        if(startDate == DateTime.MinValue)
            AddNotification("StartDate", "Start date is required.");
        
        if(startDate > DateTime.UtcNow)
            AddNotification("StartDate", "Start date cannot be in the future.");
        
        if (position != null && _positions.Any(ep => ep.PositionId == position.Id))
            AddNotification("Position", "Employee already has this position.");

        if (!IsValid)
            return;
            
        _positions.Add(new EmployeePosition(this, position, startDate));
    }

    public EmployeePosition? GetLastPosition()
    {
        if (_positions is null || !_positions.Any())
            return null;

        return _positions
            .OrderByDescending(e => e.StartDate)
            .FirstOrDefault();
    } 
}