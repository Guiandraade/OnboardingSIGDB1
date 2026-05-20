namespace OnboardingSIGDB1.Domain.Dto.Employees.Commands;

/// <summary>
/// Request payload used to change an employee position.
/// </summary>
/// <param name="PositionId">Target position identifier.</param>
public record ChangeEmployeePositionRequest(int PositionId);