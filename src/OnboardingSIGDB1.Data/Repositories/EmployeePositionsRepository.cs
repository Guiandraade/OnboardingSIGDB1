using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class EmployeePositionsRepository(OnboardingDbContext context) : BaseRepository<EmployeePosition>(context), IEmployeePositionsRepository
{
    public async Task<bool> HasEmployeeEverHeldPositionAsync(int employeeId, int positionId)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(ep =>
                ep.EmployeeId == employeeId &&
                ep.PositionId == positionId
            );
    }

    public async Task<EmployeePosition?> GetActivePositionAsync(int employeeId)
    {
        return await DbSet
            .Include(ep => ep.Employee)
            .Include(ep => ep.Position)
            .Where(ep => ep.EmployeeId == employeeId && ep.EndDate == null)
            .OrderByDescending(ep => ep.StartDate) 
            .FirstOrDefaultAsync();
    }
}