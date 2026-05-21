using FluentAssertions;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.UnitTests.Domain.Utils;

public class StringUtilsTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("abc", "")]
    [InlineData("123", "123")]
    [InlineData("a1b2c3", "123")]
    [InlineData("987.826.470-03", "98782647003")]
    [InlineData("12.345.678/0001-90", "12345678000190")]
    public void OnlyNumbers_ShouldReturnExpectedResult(string? input, string expected)
    {
        StringUtils.OnlyNumbers(input!).Should().Be(expected);
    }
}

