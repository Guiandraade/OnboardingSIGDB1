namespace OnboardingSIGDB1.Domain.Dto.Employees.Commands;

/// <summary>
/// Request payload used to create a new employee.
/// </summary>
/// <param name="Name">Employee full name.</param>
/// <param name="Cpf">Employee registration number (CPF).</param>
/// <param name="HireDate">Employee hiring date.</param>
/// <param name="CompanyId">Company identifier associated with the employee.</param>
/// <param name="PositionId">Initial position identifier.</param>
public record EmployeeRequest(
    string Name,
    string Cpf,
    DateTime? HireDate,
    int CompanyId,
    int PositionId
    );