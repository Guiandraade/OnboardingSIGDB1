namespace OnboardingSIGDB1.Domain.Dto.Common.Filters;

/// <summary>
/// Base query filter with pagination settings.
/// </summary>
public abstract class BaseFilter
{
    /// <summary>
    /// Requested page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Requested page size.
    /// </summary>
    public int PageSize { get; set; } = 10;
}