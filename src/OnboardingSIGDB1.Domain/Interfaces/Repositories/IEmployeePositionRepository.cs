using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IEmployeePositionRepository : IBaseRepository<EmployeePosition>
{ 
    Task<(IEnumerable<EmployeePosition> Data, int total)> SearchAsync(EmployeePositionFilter filter);
}