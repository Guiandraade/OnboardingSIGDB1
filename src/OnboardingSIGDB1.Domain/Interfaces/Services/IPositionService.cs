using OnboardingSIGDB1.Domain.Dto.Common.Pagination;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Commands;
using OnboardingSIGDB1.Domain.Dto.Positions.Responses;

namespace OnboardingSIGDB1.Domain.Interfaces.Services;

public interface IPositionService
{
    Task<PositionResponse?> CreateAsync(PositionRequest request);
    Task<PositionResponse?> UpdateAsync(int id, PositionRequest request);
    Task<bool> DeleteAsync(int id);
    Task<PositionResponse?> GetByIdAsync(int id);
    Task<PagedResponse<PositionResponse>> SearchAsync(PositionFilter filter);
}