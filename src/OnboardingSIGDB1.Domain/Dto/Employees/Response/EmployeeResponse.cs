namespace OnboardingSIGDB1.Domain.Dto.Employees.Response;

public class EmployeeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public DateTime? HireDate { get; set; }
    public string CompanyName { get; set; }
    public string CurrentPosition { get; set; }
}

