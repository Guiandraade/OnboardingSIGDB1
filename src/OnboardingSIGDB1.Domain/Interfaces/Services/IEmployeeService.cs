using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeResponse?> CreateAsync(EmployeeRequest request);
    Task<EmployeeResponse?> UpdateAsync(int id, EmployeeUpdateRequest request);
    Task<bool> DeleteAsync(int id);
    Task<EmployeeResponse?> GetByIdAsync(int id);
    Task<EmployeeAndPositionsResponse?> GetHistoryAsync(int id);
    Task<PagedResponse<EmployeeResponse>> SearchAsync(EmployeeFilter filter);
}

