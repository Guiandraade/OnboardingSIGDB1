using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Data.Repositories;

public class CompanyRepository(OnboardingDbContext context) : BaseRepository<Company>(context), ICompanyRepository
{
    public async Task<Company?> GetCompanyWithEmployeesByIdAsync(int id)
    {
        return await DbSet
            .AsNoTracking()
            .Include(c => c.Employees)
                .ThenInclude(e => e.Positions)
                    .ThenInclude(e  => e.Position)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Company?> GetByIdCompanyAndEmployees(int id)
    {
        return GetCompanyWithEmployeesByIdAsync(id);
    }

    public override async Task<Company?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(c => c.Employees)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Company?> GetByCnpjAsync(string cnpj)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Cnpj == cnpj);
    }

    public async Task<(IEnumerable<Company> Data, int total)> SearchAsync(CompanyFilter filter)
    {
        var query = DbSet.AsNoTracking().AsQueryable();
    
        if (!string.IsNullOrWhiteSpace(filter.Name)) 
            query = query.Where(c => c.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.Cnpj))
        {            
            var cnpjClean = StringUtils.OnlyNumbers(filter.Cnpj);
            query = query.Where(c => c.Cnpj == cnpjClean);
        }
        
        if (filter.FoundedIn.HasValue) 
            query = query.Where(c => c.FoundationDate.HasValue && c.FoundationDate.Value.Date >= filter.FoundedIn.Value.Date);
        
        if (filter.FoundedUntil.HasValue) 
            query = query.Where(c => c.FoundationDate.HasValue && c.FoundationDate.Value.Date <= filter.FoundedUntil.Value.Date);
        
        var total = await query.CountAsync();
        
        var data = await query
            .OrderBy(c => c.Name)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (data, total);
    }

    public async Task<bool> HasEmployeesAsync(int id)
    {
        return await Context.Employees.AnyAsync(e => e.CompanyId == id);
    }
    
    public async Task<DateTime?> GetEarliestEmployeeHireDateAsync(int companyId)
    {
        return await Context.Employees
            .Where(e => e.CompanyId == companyId)
            .Select(e => e.HireDate)
            .MinAsync();
    }
}   