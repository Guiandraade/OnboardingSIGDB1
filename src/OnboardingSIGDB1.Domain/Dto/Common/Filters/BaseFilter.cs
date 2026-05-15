namespace OnboardingSIGDB1.Domain.Dto.Common.Filters;

public abstract class BaseFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}