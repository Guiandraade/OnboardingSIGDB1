using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface IPositionService
{
    Task<PositionResponse?> CreateAsync(PositionRequest request);
    Task<PositionResponse?> UpdateAsync(int id, PositionRequest request);
    Task<bool> DeleteAsync(int id);
    Task<PositionResponse?> GetByIdAsync(int id);
    Task<PagedResponse<PositionResponse>> SearchAsync(PositionFilter filter);
}