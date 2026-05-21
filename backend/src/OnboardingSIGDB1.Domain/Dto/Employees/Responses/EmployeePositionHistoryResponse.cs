namespace OnboardingSIGDB1.Domain.Dto.Employees.Responses;

/// <summary>
/// Historical position entry associated with an employee.
/// </summary>
public class EmployeePositionHistoryResponse
{
    /// <summary>
    /// Position name.
    /// </summary>
    public string PositionName { get; set; } = string.Empty;

    /// <summary>
    /// Date when the employee started in the position.
    /// </summary>
    public DateTime StartDate { get; set; }
}