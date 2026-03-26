using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class EmployeePositionRepository(OnboardingDbContext context) : BaseRepository<EmployeePosition>(context), IEmployeePositionRepository
{
    public async Task<IEnumerable<EmployeePosition>> SearchAsync(EmployeePositionFilter filter)
    {
        var query = DbSet.AsNoTracking()
            .Include(ep => ep.Employee)
            .Include(ep => ep.Position)
            .AsQueryable();

        if (filter.EmployeeId.HasValue)
            query = query.Where(ep => ep.EmployeeId == filter.EmployeeId.Value);

        if (filter.PositionId.HasValue)
            query = query.Where(ep => ep.PositionId == filter.PositionId.Value);
        
        if (filter.StartDateFrom.HasValue)
            query = query.Where(ep => ep.StartDate >= filter.StartDateFrom.Value);

        if (filter.StartDateUntil.HasValue)
            query = query.Where(ep => ep.StartDate <= filter.StartDateUntil.Value);

        int skip = (filter.PageNumber - 1) * filter.PageSize;

        return await query
            .OrderByDescending(ep => ep.StartDate)
            .Skip(skip)
            .Take(filter.PageSize)
            .ToListAsync();
    }
}