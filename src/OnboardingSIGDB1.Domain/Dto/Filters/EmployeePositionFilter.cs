namespace OnboardingSIGDB1.Domain.Dto.filters;

public class EmployeePositionFilter : BaseFilter
{
    public int? EmployeeId { get; set; }
    public int? PositionId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateUntil { get; set; }
}