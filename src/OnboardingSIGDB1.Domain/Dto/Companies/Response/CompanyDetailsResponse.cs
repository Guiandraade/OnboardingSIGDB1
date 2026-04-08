namespace OnboardingSIGDB1.Domain.Dto.Companies.Response;

public record CompanyDetailsResponse
{
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string PositionName { get; init; } = string.Empty;
    public DateTime HiringDate { get; init; }
}