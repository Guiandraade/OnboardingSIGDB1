namespace OnboardingSIGDB1.Domain.Dto.Employees.Responses;

/// <summary>
/// Response payload representing an employee with position history.
/// </summary>
public record EmployeeAndPositionsResponse
{
    /// <summary>
    /// Employee identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Employee registration number (CPF).
    /// </summary>
    public string Cpf { get; init; } = string.Empty;

    /// <summary>
    /// Employee hiring date.
    /// </summary>
    public DateTime? HireDate { get; init; }

    /// <summary>
    /// Company name associated with the employee.
    /// </summary>
    public string CompanyName { get; init; } = string.Empty;

    /// <summary>
    /// Current employee position.
    /// </summary>
    public string CurrentPosition { get; init; } = string.Empty;
    
    /// <summary>
    /// Historical positions held by the employee.
    /// </summary>
    public List<EmployeePositionHistoryResponse> PositionHistory { get; init; } = new();
}