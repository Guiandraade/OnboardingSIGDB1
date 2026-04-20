using FluentAssertions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.Position;

public class PositionValidationTests
{
    [Fact]
    public void ShouldBeValidWhenAllPropertiesAreValid()
    {
        var ps = PositionBuilder.New().Build();

        var result = ps.Validation();

        result.Should().BeTrue();
        ps.ValidationResult.Errors.Should().BeEmpty();
    }
    
    [Theory]
    [InlineData("te")]
    [InlineData("")]
    public void ShouldFailWhenDescriptionIsLessThanThreeOrEmpty(string descricao)
    {
        var ps = PositionBuilder.New()
            .WithDescription(descricao)
            .Build();

        var result = ps.Validation();
        result.Should().BeFalse();
        
        var propertiesWithErrors = ps.ValidationResult.Errors
            .Select(e => e.PropertyName)
            .ToList();

        propertiesWithErrors.Should().Contain(new[]
        {
            nameof(ps.Description)
        });
    }
    
    [Fact]
    public void Validation_ShouldBeIdempotent_WhenCalledTwice()
    {
        var ps = PositionBuilder.New()
            .WithDescription("")
            .Build();
        
        var firstValid = ps.Validation();
        var firstErrors = ps.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        var secondValid = ps.Validation();
        var secondErrors = ps.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        firstValid.Should().BeFalse();
        secondValid.Should().BeFalse();
        secondErrors.Should().BeEquivalentTo(firstErrors);
    }
}