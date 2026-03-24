using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Entities;

public class Position : Notifiable
{
    public int Id { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<EmployeePosition> _employeePositions = new();
    public IReadOnlyCollection<EmployeePosition> EmployeePositions => _employeePositions.AsReadOnly();

    protected Position() { }

    public Position(string description)
    {
        SetDescription(description);
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string newDescription) => SetDescription(newDescription);
    
    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            AddNotification("Description", "Description is required.");
        else if (description.Length > 250)
            AddNotification("Description", "The job description should not exceed 250 characters.");
        else 
            Description = description;
    }
}