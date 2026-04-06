using FluentValidation;
using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.Entities.Positions;

public class Position :  BaseEntity<Position>
{
    public string Description { get; private set; }
    
    public ValidationResult ValidationResult { get; private set; }
    
    private readonly List<EmployeePosition> _employeePositions = new();
    public IReadOnlyCollection<EmployeePosition> EmployeePositions => _employeePositions.AsReadOnly();
    
    protected Position() { }

    public Position(string description)
    {
        Description = description.Trim();
    }

    public void Update(string description)
    {
        Description = description.Trim();
        Validation();
    }

    public override bool Validation()
    {
        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(3).WithMessage("Description must have at least 3 characters") 
            .MaximumLength(100).WithMessage("Description cannot exceed 100 characters"); 

        ApplyValidation(this);
        
        return IsValid;
    }
}
