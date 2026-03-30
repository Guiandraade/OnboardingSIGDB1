using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IEmployeeRepository : IBaseRepository<Employee>
{
    Task<Employee?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId);
    Task<(IEnumerable<Employee> Data, int total)> SearchAsync(EmployeeFilter filter);
}