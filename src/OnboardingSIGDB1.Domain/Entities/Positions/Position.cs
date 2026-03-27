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
        Description = description;
    }

    public override bool Validation()
    {
        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Description is required");

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
}
