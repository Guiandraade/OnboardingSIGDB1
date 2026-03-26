using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities.Companies;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories; 

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task<IEnumerable<Company>> SearchAsync(CompanyFilter filter);
    Task<bool> HasEmployeesAsync(int id);
}