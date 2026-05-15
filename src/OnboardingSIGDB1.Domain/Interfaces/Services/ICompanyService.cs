using OnboardingSIGDB1.Domain.Dto.Companies.Commands;
using OnboardingSIGDB1.Domain.Dto.Companies.Responses;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Common.Pagination;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface ICompanyService
{
    Task<CompanyResponse?> CreateAsync(CompanyRequest request);
    Task<CompanyResponse?> UpdateAsync(int id, CompanyRequest request);
    Task<bool> DeleteAsync(int id);
    Task<CompanyResponse?> GetByIdAsync(int id);
    Task<PagedResponse<CompanyResponse>> SearchAsync(CompanyFilter filter);
    Task<CompanyAndEmployeesResponse?> GetCompanyWithEmployeesByIdAsync(int id);
    [Obsolete("Use GetCompanyWithEmployeesByIdAsync instead.")]
    Task<CompanyAndEmployeesResponse?> GetByIdCompanyAndEmployees(int id);
}