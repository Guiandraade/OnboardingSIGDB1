using FluentAssertions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.Positions;

public class PositionBehaviorTests
{
    [Fact]
    public void Constructor_ShouldTrimDescription()
    {
        var ps = PositionBuilder.New()
            .WithDescription(" Test Description ")
            .Build();
        
        ps.Description.Should().Be("Test Description");
    }
    
    [Fact]
    public void Update_ShouldTrimDescription()
    {
        var ps = PositionBuilder.New().Build();
        
        ps.Update(" Updated Description ");
        
        ps.Description.Should().Be("Updated Description");
    }
}