using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Entities;

public class EmployeePosition : BaseEntity
{
    
    public Employee Employee { get; private set; }
    public int EmployeeId { get; private set; }
    
    public Position Position { get; private set; }
    public int PositionId { get; private set; }
    
    public DateTime? StartDate { get; private set; }

    protected EmployeePosition() { }
    
    public EmployeePosition(Employee employee, Position position, DateTime? startDate)
    {
        SetEmployee(employee);
        SetPosition(position);
        SetStartDate(startDate);
        CreatedAt = DateTime.UtcNow;
    }

    private void SetEmployee(Employee employee)
    {
        if (employee is null)
        {
            AddNotification("Employee", "The employee cannot be null.");
            return;
        }
        
        EmployeeId =  employee.Id;
        Employee = employee;
    }

    private void SetPosition(Position position)
    {
        if (position is null)
        {
            AddNotification("Position", "The position cannot be null.");
            return;
        }

        PositionId = position.Id;
        Position = position;
    }

    private void SetStartDate(DateTime? startDate)
    {
        if (startDate.HasValue && startDate > DateTime.UtcNow)
            AddNotification("StartDate", "The start date cannot be in the future.");
        else
            StartDate = startDate;
    }
}