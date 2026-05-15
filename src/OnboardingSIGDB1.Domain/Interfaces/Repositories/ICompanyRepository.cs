using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Entities.Companies;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories; 

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task<(IEnumerable<Company> Data, int total)> SearchAsync(CompanyFilter filter);
    Task<bool> HasEmployeesAsync(int id);
    Task<Company?> GetByIdCompanyAndEmployees(int id);
    Task<DateTime?> GetEarliestEmployeeHireDateAsync(int companyId);
}