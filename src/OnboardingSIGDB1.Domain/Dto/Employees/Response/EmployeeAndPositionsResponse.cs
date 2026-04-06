namespace OnboardingSIGDB1.Domain.Dto.Employees.Response;

public class EmployeeAndPositionsResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public DateTime? HireDate { get; set; }
    public string CompanyName { get; set; }
    public string CurrentPosition { get; set; }
    
    public List<EmployeePositionHistoryResponse> PositionHistory { get; set; } = new();
}