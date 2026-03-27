using FluentValidation;
using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Entities.Positions;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace OnboardingSIGDB1.Domain.Entities.Employees;

public class EmployeePosition : BaseElement<EmployeePosition>
{
    public Employee Employee { get; private set; }
    public int EmployeeId { get; private set; }
    
    public Position Position { get; private set; }
    public int PositionId { get; private set; }
    
    public DateTime StartDate { get; private set; }
    
    public ValidationResult ValidationResult { get; private set; }

    protected EmployeePosition() { }
    
    public EmployeePosition(Employee employee, Position position, DateTime startDate)
    {
       Employee = employee;
       EmployeeId = employee.Id;
       Position = position;
       PositionId = position.Id;
       StartDate = startDate;
    }

    public override bool Validation()
    {
        ClearNotifications();

        RuleFor(ep => ep.EmployeeId)
            .GreaterThan(0).WithMessage("Employee id must be greater than 0");
        
        RuleFor(ep => ep.PositionId)
            .GreaterThan(0).WithMessage("Position id must be greater than 0");
            
        RuleFor(ep => ep.StartDate)
            .NotEmpty().WithMessage("Start date must not be empty")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("The start date cannot be in the future.");
        
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