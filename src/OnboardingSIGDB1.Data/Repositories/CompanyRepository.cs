using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class CompanyRepository(OnboardingDbContext context) : BaseRepository<Company>(context), ICompanyRepository
{
    public async Task<Company?> GetByCnpjAsync(string cnpj)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
    }

    public async Task<IEnumerable<Company>> SearchAsync(CompanyFilter filter)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name)) 
            query = query.Where(c => c.Name.Contains(filter.Name));
        
        if(!string.IsNullOrWhiteSpace(filter.Cnpj)) 
            query = query.Where(c => c.Cnpj == filter.Cnpj);
        
        if (filter.FoundedIn.HasValue) 
            query = query.Where(c => c.FoundationDate >= filter.FoundedIn.Value);
        
        if (filter.FoundedUntil.HasValue) 
            query = query.Where(c => c.FoundationDate <= filter.FoundedUntil.Value);
        
        int skip = (filter.PageNumber - 1) * filter.PageSize;
        
        return await query
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<bool> HasEmployeesAsync(int id)
    {
        return await Context.Employees.AnyAsync(e => e.CompanyId == id);
    }
}   