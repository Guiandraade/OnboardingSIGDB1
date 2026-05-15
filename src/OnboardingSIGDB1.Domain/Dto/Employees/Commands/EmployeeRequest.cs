namespace OnboardingSIGDB1.Domain.Dto.Employees.Commands;

public record EmployeeRequest(
    string Name,
    string Cpf,
    DateTime? HireDate,
    int CompanyId,
    int PositionId
    );