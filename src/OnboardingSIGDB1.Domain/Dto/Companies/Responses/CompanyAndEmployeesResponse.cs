namespace OnboardingSIGDB1.Domain.Dto.Companies.Responses;

public record CompanyAndEmployeesResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;
    public DateTime? FoundationDate { get; init; }
    public List<CompanyDetailsResponse> EmployeesPositionHistory { get; init; } = new();
}