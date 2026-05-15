using OnboardingSIGDB1.Domain.Dto.Common.Pagination;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeResponse?> CreateAsync(EmployeeRequest request);
    Task<EmployeeResponse?> UpdateAsync(int id, EmployeeUpdateRequest request);
    Task<bool> DeleteAsync(int id);
    Task<EmployeeResponse?> GetByIdAsync(int id);
    Task<EmployeeAndPositionsResponse?> GetHistoryAsync(int id);
    Task<PagedResponse<EmployeeResponse>> SearchAsync(EmployeeFilter filter);
    Task<bool> ChangePositionAsync(int employeeId, ChangeEmployeePositionRequest request);
}

