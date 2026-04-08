namespace OnboardingSIGDB1.Domain.Dto.Positions.Response;

public record PositionResponse
{
    public int Id { get; init; }
    public string Description { get; init; } = string.Empty;
}