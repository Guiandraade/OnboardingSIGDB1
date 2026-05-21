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
    public DateTime? EndDate { get; private set; }
    
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

    public void ClosePosition(DateTime endDate)
    {
        EndDate = endDate;
    }
    
    public override bool Validation()
    {
        if (!RulesRegistered)
        {
            RuleFor(ep => ep.Employee)
                .NotNull()
                .WithMessage("Employee is required.");

            RuleFor(ep => ep.Position)
                .NotNull()
                .WithMessage("Position is required.");

            RuleFor(ep => ep.PositionId)
                .GreaterThan(0)
                .WithMessage("Position id must be greater than 0.");

            RuleFor(ep => ep.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.")
                .Must(d => d.Date <= DateTime.UtcNow.Date)
                .WithMessage("Start date cannot be in the future.")
                .Must(d => d > new DateTime(1900, 1, 1))
                .WithMessage("The start date must be after 01/01/1900.");

            RuleFor(ep => ep.EndDate)
                .Must((ep, endDate) => !endDate.HasValue || endDate >= ep.StartDate)
                .WithMessage("The end date cannot be earlier than the start date.");

            MarkRulesAsRegistered();
        }

        ValidationResult = Validate(this);
        return ValidationResult.IsValid;
    }
    
}