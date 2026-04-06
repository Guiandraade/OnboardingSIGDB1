namespace OnboardingSIGDB1.Domain.Dto.Filters;

public abstract class BaseFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}