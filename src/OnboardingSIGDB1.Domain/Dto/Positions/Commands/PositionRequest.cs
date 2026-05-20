namespace OnboardingSIGDB1.Domain.Dto.Positions.Commands;

/// <summary>
/// Request payload used to create or update a position.
/// </summary>
/// <param name="Description">Position description.</param>
public record PositionRequest(
    string Description
);