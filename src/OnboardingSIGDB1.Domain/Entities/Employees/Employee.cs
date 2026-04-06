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
    
    public string CurrentPositionDescription => 
        GetLastPosition()?.Position?.Description ?? "No link";
    
    public ValidationResult ValidationResult { get; private set; }
    
    private readonly List<EmployeePosition> _positions = new();
    public IReadOnlyCollection<EmployeePosition> Positions => _positions.AsReadOnly();

    protected Employee(){}
    
    public Employee(string name, string cpf, DateTime? hireDate, int companyId, Position position)
    {
        Name = name;
        Cpf = StringUtils.OnlyNumbers(cpf ?? "");
        HireDate = hireDate;
        CompanyId = companyId;

        if (companyId <= 0 || position == null)
        {
            if (companyId <= 0) 
                AddNotification("CompanyId", "Company is required.");
            
            if (position == null) 
                AddNotification("Position", "Initial position is required.");
            
            return;
        }
        
        AddPosition(position, hireDate ?? DateTime.UtcNow);
    }

    public void Update(string name, string cpf, DateTime? hireDate, Position position)
    {
        Name = name;
        
        if(cpf != null)
            Cpf = StringUtils.OnlyNumbers(cpf);
        
        var lastPosition = GetLastPosition();
        
        if (position != null &&
            (lastPosition == null || lastPosition.PositionId != position.Id))   
        {
            AddPosition(position, hireDate ?? DateTime.UtcNow);
        }
        
        Validation();
    }
    public override bool Validation()
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("The name must have at least 3 characters.") 
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
        
        RuleFor(e => e.Cpf)
            .NotEmpty().WithMessage("CPF is required.")
            .Length(11).WithMessage("CPF must not exceed 11 characters.")
            .Must(CpfValidator.IsValid).WithMessage("The CPF provided is invalid.");
        
        RuleFor(e => e.HireDate)
            .Must(d => !d.HasValue || (d.Value > DateTime.MinValue && d.Value <= DateTime.UtcNow))
            .WithMessage("Invalid hiring data.");

        RuleFor(e => e.CompanyId)
            .GreaterThan(0).WithMessage("CompanyId is required.");
        
        ApplyValidation(this);
        
        return IsValid;
    }
    
    private void AddPosition(Position position, DateTime startDate)
    {
        if (position == null)
        {
            AddNotification("Position", "Position is required.");
            return;
        }
        
        var newPosition = new EmployeePosition(this, position, startDate);
        
        if (!newPosition.Validation())
        {
            AddNotifications(newPosition.Notifications);
            return;
        }
        
        _positions.Add(newPosition);
    }

    public EmployeePosition? GetLastPosition()
    {
        if (!_positions.Any())
            return null;

        return _positions
            .OrderByDescending(e => e.StartDate)
            .FirstOrDefault();
    }
}