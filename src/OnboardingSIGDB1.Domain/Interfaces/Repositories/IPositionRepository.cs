using OnboardingSIGDB1.Domain.Dto.filters;
using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IPositionRepository : IBaseRepository<Position>
{
    Task<Position?> GetByDescriptionAsync(string description);
    Task<IEnumerable<Position>> SearchAsync(PositionFilter filter);
}