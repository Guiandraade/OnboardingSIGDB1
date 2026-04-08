using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.EmployeeAndPositions.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface IEmployeeService
{
    Task<Result<EmployeeResponse>> CreateAsync(EmployeeRequest request);
    Task<Result<EmployeeResponse>> UpdateAsync(int id, EmployeeUpdateRequest request);
    Task<Result> DeleteAsync(int id);
    Task<Result<EmployeeResponse>> GetByIdAsync(int id);
    Task<Result<EmployeeAndPositionsResponse>> GetHistoryAsync(int id);
    Task<Result<PagedResponse<EmployeeResponse>>> SearchAsync(EmployeeFilter filter);
    Task<Result> ChangePositionAsync(int employeeId, ChangeEmployeePositionRequest request);
}

