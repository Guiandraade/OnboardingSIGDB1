namespace OnboardingSIGDB1.Domain.Dto.Employees.Commands;

/// <summary>
/// Request payload used to update an existing employee.
/// </summary>
/// <param name="Name">Employee full name.</param>
/// <param name="Cpf">Employee registration number (CPF).</param>
public record EmployeeUpdateRequest(
    string Name,
    string Cpf
);