namespace OnboardingSIGDB1.Domain.Dto.Positions.Responses;

/// <summary>
/// Response payload representing a position.
/// </summary>
public record PositionResponse
{
    /// <summary>
    /// Position identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Position description.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}