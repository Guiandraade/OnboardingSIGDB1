using FluentAssertions;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.UnitTests.Domain.Utils;

public class CnpjValidatorTests
{
    [Theory]
    [InlineData("41977659000180")] // valid CNPJ
    [InlineData("11222333000181")] // valid CNPJ
    [InlineData("27284997000105")] // valid CNPJ
    [InlineData("19283746000188")] // valid CNPJ
    public void ShouldReturnTrueForValidCnpj(string cnpj)
    {
        CnpjValidator.IsValid(cnpj).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void ShouldReturnFalseForNullOrEmptyCnpj(string cnpj)
    {
        CnpjValidator.IsValid(cnpj).Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenAllDigitsAreEqual()
    {
        CnpjValidator.IsValid("11111111111111").Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenLengthIsNot14()
    {
        CnpjValidator.IsValid("1234567890123").Should().BeFalse();  // 13 digits
        CnpjValidator.IsValid("123456789012345").Should().BeFalse(); // 15 digits
    }
    
    [Fact]
    public void ShouldReturnFalseWhenFirstDigitIsCorrectButSecondIsWrong()
    {
        // 11222333000181 is valid. Change last digit (1→2) → only second digit wrong
        CnpjValidator.IsValid("11222333000182").Should().BeFalse();
    }
    
    [Fact]
    public void ShouldReturnFalseWhenFirstDigitIsWrongButSecondIsCorrect()
    {
        // 11222333000181 is valid (digits 8,1). Change first check digit (8→0) → first digit wrong
        CnpjValidator.IsValid("11222333000101").Should().BeFalse();
    }
    
    /// <summary>
    /// CNPJs where remainder == 2 in check digit calculation (d = 11-2 = 9).
    /// Kills mutant: remainder &lt; 2 → remainder &lt;= 2.
    /// </summary>
    [Theory]
    [InlineData("10000000000498")] // r1=2, d1=9 — kills mutant < 2 → <= 2 on first digit
    [InlineData("30000000000829")] // r2=2, d2=9 — kills mutant < 2 → <= 2 on second digit
    public void ShouldValidateCnpjWithRemainderEqualTo2(string cnpj)
    {
        CnpjValidator.IsValid(cnpj).Should().BeTrue();
    }
    
    [Theory]
    [InlineData("11222333000199")] // both digits wrong
    [InlineData("41977659000199")] // both digits wrong
    [InlineData("11222333000100")] // both digits zero
    public void ShouldReturnFalseForInvalidCnpj(string cnpj)
    {
        CnpjValidator.IsValid(cnpj).Should().BeFalse();
    }
}
