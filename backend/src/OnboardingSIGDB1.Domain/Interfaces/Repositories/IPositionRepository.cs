using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IPositionRepository : IBaseRepository<Position>
{
    Task<Position?> GetByDescriptionAsync(string description);
    Task<bool> HasEmployeesAsync(int id);
    Task<(IEnumerable<Position> Data, int total)> SearchAsync(PositionFilter filter);
}