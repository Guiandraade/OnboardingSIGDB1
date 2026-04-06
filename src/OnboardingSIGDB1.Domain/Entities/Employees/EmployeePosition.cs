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
       EmployeeId = employee?.Id ?? 0;
       
       Position = position;
       PositionId = position?.Id ?? 0;
       
       StartDate = startDate;
    }

    public override bool Validation()
    {
        ClearNotifications();

        RuleFor(ep => ep.Position)
            .NotNull().WithMessage("Position is required.");
        
        RuleFor(ep => ep.Employee)
            .NotNull().WithMessage("Employee is required.");
        
        RuleFor(ep => ep.EmployeeId)
            .GreaterThan(0).WithMessage("EmployeeId must be greater than 0");
        
        RuleFor(ep => ep.PositionId)
            .GreaterThan(0).WithMessage("Position id must be greater than 0");
            
        RuleFor(ep => ep.StartDate)
            .Must(d => d > DateTime.MinValue)
            .WithMessage("Start date must be a valid date.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The start date cannot be in the future.");
        
        ApplyValidation(this);
        
        return IsValid;
    }
}