using FluentAssertions;
using OnboardingSIGDB1.Domain.Dto.Base;

namespace OnboardingSIGDB1.UnitTests.Domain.Dto;

public class PagedResponseTests
{
    [Fact]
    public void DefaultConstructor_SetsDataToEmpty()
    {
        var response = new PagedResponse<string>();

        response.Data.Should().NotBeNull();
        response.Data.Should().BeEmpty();
        response.Total.Should().Be(0);
        response.PageNumber.Should().Be(0);
        response.PageSize.Should().Be(0);
    }

    [Fact]
    public void ParameterizedConstructor_SetsAllProperties()
    {
        var data = new[] { "A", "B" };

        var response = new PagedResponse<string>(data, 100, 2, 25);

        response.Data.Should().BeEquivalentTo(data);
        response.Total.Should().Be(100);
        response.PageNumber.Should().Be(2);
        response.PageSize.Should().Be(25);
    }
}

