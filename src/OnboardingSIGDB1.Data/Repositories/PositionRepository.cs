using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class PositionRepository(OnboardingDbContext context) : BaseRepository<Position>(context), IPositionRepository
{
    public async Task<Position?> GetByDescriptionAsync(string description)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Description == description);
    }

    public async Task<bool> HasEmployeesAsync(int id)
    {
        return await Context
            .EmployeePositions
            .AsNoTracking()
            .AnyAsync(e => e.PositionId == id);
    }
    
    public async Task<(IEnumerable<Position> Data, int total)> SearchAsync(PositionFilter filter)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Description))
            query = query.Where(p => p.Description.Contains(filter.Description));
        
        var total = await query.CountAsync();
        
        var data = await query
            .OrderBy(p => p.Description)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
        
        return (data, total);
    }
}