namespace OnboardingSIGDB1.Domain.Dto.Employees.Commands;

public record EmployeeUpdateRequest(
    string Name,
    string Cpf
);