using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class EmployeeRepository(OnboardingDbContext context) : BaseRepository<Employee>(context), IEmployeeRepository
{
    public async Task<Employee?> GetByCpfAsync(string cpf)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Cpf == cpf);
    }

    public async Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId)
    {
        return await DbSet.AsNoTracking()
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> SearchAsync(EmployeeFilter filter)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(e => e.Name.Contains(filter.Name));
        
        if(!string.IsNullOrWhiteSpace(filter.Cpf))
            query = query.Where(e => e.Cpf == filter.Cpf);
        
        if(filter.HiredFrom.HasValue)
            query = query.Where(e => e.HireDate >= filter.HiredFrom.Value);
        
        if(filter.Hireduntil.HasValue)
            query = query.Where(e => e.HireDate <= filter.Hireduntil.Value);

        int skip = (filter.PageNumber - 1) * filter.PageSize;
        
        return await query
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(filter.PageSize)
            .ToListAsync();
    }
}