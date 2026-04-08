namespace OnboardingSIGDB1.Domain.Dto.EmployeeAndPositions.Response;

public record PositionHistoryItemResponse
{
    public string PositionDescription { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}