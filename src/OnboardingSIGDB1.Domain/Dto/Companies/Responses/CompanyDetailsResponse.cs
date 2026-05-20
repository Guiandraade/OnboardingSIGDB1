namespace OnboardingSIGDB1.Domain.Dto.Companies.Responses;

/// <summary>
/// Employee position history entry returned within a company details response.
/// </summary>
public record CompanyDetailsResponse
{
    /// <summary>
    /// Employee identifier.
    /// </summary>
    public int EmployeeId { get; init; }

    /// <summary>
    /// Employee full name.
    /// </summary>
    public string EmployeeName { get; init; } = string.Empty;

    /// <summary>
    /// Position name held by the employee.
    /// </summary>
    public string PositionName { get; init; } = string.Empty;

    /// <summary>
    /// Employee hiring date.
    /// </summary>
    public DateTime HiringDate { get; init; }
}