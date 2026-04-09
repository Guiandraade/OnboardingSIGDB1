using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Data.Repositories;

public class EmployeeRepository(OnboardingDbContext context) : BaseRepository<Employee>(context), IEmployeeRepository
{
    public override async Task<Employee?> GetByIdAsync(int id)
    {
        return await DbSet
            .Include(e => e.Company)
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<Employee?> GetHistoryAsync(int id)
    {
        return await DbSet
            .Include(e => e.Company)
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByIdWithCompanyAsync(int id)
    {
        return await DbSet
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByCpfAsync(string cpf)
    {
        var cpfClean = StringUtils.OnlyNumbers(cpf);
        return await DbSet.FirstOrDefaultAsync(c => c.Cpf == cpfClean);
    }

    public async Task<(IEnumerable<Employee> Data, int total)> SearchAsync(EmployeeFilter filter)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(e => e.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.Cpf))
        {
            var cpfClean = StringUtils.OnlyNumbers(filter.Cpf);
            query = query.Where(e => e.Cpf == cpfClean);
        }
        
        if(filter.HiredFrom.HasValue)
            query = query.Where(e => e.HireDate >= filter.HiredFrom.Value);
        
        if(filter.HiredUntil.HasValue)
            query = query.Where(e => e.HireDate <= filter.HiredUntil.Value);
        
        var total = await query.CountAsync();

        var data = await query
            .Include(e => e.Company)
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .OrderBy(e => e.Name)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (data, total);
        
    }
}