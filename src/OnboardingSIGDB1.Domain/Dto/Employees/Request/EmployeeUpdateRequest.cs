namespace OnboardingSIGDB1.Domain.Dto.Employees.Request;

public record EmployeeUpdateRequest(
    string Name,
    string Cpf,
    DateTime? HireDate,
    int PositionId
);

    
