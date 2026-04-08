namespace OnboardingSIGDB1.Domain.Dto.Employees.Response;

public record EmployeeAndPositionsResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public DateTime? HireDate { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string CurrentPosition { get; init; } = string.Empty;
    
    public List<EmployeePositionHistoryResponse> PositionHistory { get; init; } = new();
}