using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IEmployeeRepository : IBaseRepository<Employee>
{
    Task<Employee?> GetByCpfAsync(string cpf);
    Task<Employee?> GetHistoryAsync(int id);
    Task<(IEnumerable<Employee> Data, int total)> SearchAsync(EmployeeFilter filter);
}