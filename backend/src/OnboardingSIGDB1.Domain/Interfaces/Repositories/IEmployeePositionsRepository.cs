using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IEmployeePositionsRepository : IBaseRepository<EmployeePosition>
{
    Task<bool> HasEmployeeEverHeldPositionAsync(int employeeId, int positionId);
    Task<EmployeePosition?> GetActivePositionAsync(int employeeId);
}