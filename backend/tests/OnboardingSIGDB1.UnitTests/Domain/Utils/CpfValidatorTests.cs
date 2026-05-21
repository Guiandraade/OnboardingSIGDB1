using FluentAssertions;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.UnitTests.Domain.Utils;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("70720722098")] // valid CPF
    [InlineData("98782647003")] // valid CPF  
    [InlineData("34226696042")] // valid CPF
    public void ShouldReturnTrueForValidCpf(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void ShouldReturnFalseForNullOrEmptyCpf(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenAllDigitsAreEqual()
    {
        CpfValidator.IsValid("11111111111").Should().BeFalse();
        CpfValidator.IsValid("22222222222").Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenLengthIsNot11()
    {
        CpfValidator.IsValid("1234567890").Should().BeFalse();   // 10 digits
        CpfValidator.IsValid("123456789012").Should().BeFalse(); // 12 digits
    }
    
    [Fact]
    public void ShouldReturnFalseWhenFirstDigitIsCorrectButSecondIsWrong()
    {
        // 70720722098 is valid. Change last digit (8→1) → second digit wrong.
        CpfValidator.IsValid("70720722091").Should().BeFalse();
    }
    
    [Fact]
    public void ShouldReturnFalseWhenFirstDigitIsWrongButSecondIsCorrect()
    {
        // 70720722098 is valid (check digits 9, 8). Change first check digit (9→0).
        CpfValidator.IsValid("70720722008").Should().BeFalse();
    }
    
    [Fact]
    public void ShouldReturnFalseForCpfWith11CharsIncludingNonDigits()
    {
        // "1234567890a" → OnlyNumbers → "1234567890" (10 digits) → length != 11 → false
        CpfValidator.IsValid("1234567890a").Should().BeFalse();
    }
    
    [Theory]
    [InlineData("98782647099")] // both digits wrong
    [InlineData("70720722099")] // both digits wrong
    public void ShouldReturnFalseForCpfWithBothDigitsWrong(string cpf)
    {
        CpfValidator.IsValid(cpf).Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnTrueWhenSecondDigitNormalizesFromTenToZero()
    {
        CpfValidator.IsValid("00000001830").Should().BeTrue();
    }
}
