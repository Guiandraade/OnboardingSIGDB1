using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.UnitTests.Builders;

public class PositionBuilder
{
    private string _description = "Developer";

    public static PositionBuilder New() => new();

    public PositionBuilder WithDescription(string description) { _description = description; return this; }

    public Position Build() => new(_description);
}

