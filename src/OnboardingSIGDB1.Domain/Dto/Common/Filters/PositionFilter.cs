
namespace OnboardingSIGDB1.Domain.Dto.Common.Filters;

/// <summary>
/// Query filter used to search positions.
/// </summary>
public class PositionFilter : BaseFilter
{
    /// <summary>
    /// Filters positions by description.
    /// </summary>
    public string? Description { get; set; }
}