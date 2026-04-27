using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class EmployeePositionRepository(OnboardingDbContext context) : BaseRepository<EmployeePosition>(context), IEmployeePositionRepository
{
    public async Task<(IEnumerable<EmployeePosition> Data, int total)> SearchAsync(EmployeePositionFilter filter)
    {
        var query = DbSet.AsNoTracking().AsQueryable();
        
        if (filter.EmployeeId.HasValue)
            query = query.Where(ep => ep.EmployeeId == filter.EmployeeId.Value);

        if (filter.PositionId.HasValue)
            query = query.Where(ep => ep.PositionId == filter.PositionId.Value);
        
        if (filter.StartDateFrom.HasValue)
            query = query.Where(ep => ep.StartDate >= filter.StartDateFrom.Value);

        if (filter.StartDateUntil.HasValue)
            query = query.Where(ep => ep.StartDate <= filter.StartDateUntil.Value);

        int skip = (filter.PageNumber - 1) * filter.PageSize;
        
        var total = await query.CountAsync();
        
        var data = await query
            .Include(ep => ep.Employee)
            .Include(ep => ep.Position)
            .OrderByDescending(ep => ep.StartDate)
            .Skip(skip)
            .Take(filter.PageSize)
            .ToListAsync();

        return (data, total);
        
    }
}