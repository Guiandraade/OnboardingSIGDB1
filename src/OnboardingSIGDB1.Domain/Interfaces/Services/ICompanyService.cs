using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface ICompanyService
{
    Task<CompanyResponse?> CreateAsync(CompanyRequest request);
    Task<CompanyResponse?> UpdateAsync(int id, CompanyRequest request);
    Task<bool> DeleteAsync(int id);
    Task<CompanyResponse?> GetByIdAsync(int id);
    Task<PagedResponse<CompanyResponse>> SearchAsync(CompanyFilter filter);
    Task<CompanyAndEmployeesResponse?> GetByIdCompanyAndEmployees(int id);
}