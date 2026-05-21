using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.UnitTests.Builders;

public class PositionBuilder
{
    private string _description = "Developer";
    private int? _id;

    public static PositionBuilder New() => new();

    public PositionBuilder WithDescription(string description) { _description = description; return this; }
    public PositionBuilder WithId(int id) { _id = id; return this; }

    public Position Build()
    {
        var position = new Position(_description);

        if (_id.HasValue)
        {
            var prop = position.GetType().GetProperty("Id");
            prop?.SetValue(position, _id.Value);
        }

        return position;
    }
}

