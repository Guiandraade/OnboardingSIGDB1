using FluentAssertions;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.Companies;

public class CompanyValidationTests
{
    [Fact]
    public void ShouldBeValidWhenAllPropertiesAreValid()
    {
        var company = CompanyBuilder.New().Build();

        var result = company.Validation();
        
        result.Should().BeTrue();
        company.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldContainThreeValidationErrors()
    {
        var company = CompanyBuilder.New()
            .WithName("DB")
            .WithCnpj("")
            .WithFoundationDate(DateTime.UtcNow.AddDays(1))
            .Build();
        
        var result = company.Validation();
        result.Should().BeFalse();

        var propertiesWithErrors = company.ValidationResult.Errors
            .Select(e => e.PropertyName)
            .ToList();
        
        propertiesWithErrors.Should().Contain(new[]
        {
            nameof(Company.Name),
            nameof(Company.Cnpj),
            nameof(Company.FoundationDate)
        });
    }

    [Theory]
    [InlineData("Te")]
    [InlineData("")]
    public void ShouldFailWhenNameIsLessThanThreeOrEmpty(string name)
    {
        var company = CompanyBuilder.New()
            .WithName(name)
            .Build();

        var result = company.Validation();

        result.Should().BeFalse();
        company.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Company.Name));
    }

    [Fact]
    public void ShouldFailWhenNameExceeds150Characters()
    {
        var company = CompanyBuilder.New()
            .WithName(new string('A', 151))
            .Build();

        var result = company.Validation();
        result.Should().BeFalse();

        company.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Company.Name));
    }
    
    [Theory]
    [InlineData("1111111111111")]
    [InlineData("111111111111111")]
    [InlineData("11111")]
    public void ShouldFailWhenCnpjIsNot14Digits(string cnpj)
    {
        var company = CompanyBuilder.New()
            .WithCnpj(cnpj)
            .Build();

        var result = company.Validation();

        result.Should().BeFalse();
        company.ValidationResult.Errors
            .Should().ContainSingle(e => e.PropertyName == nameof(Company.Cnpj) && 
                                         e.ErrorMessage.Contains("14"));
    }
    
    [Fact]
    public void ShouldFailWhenCnpjIsEmpty()
    {
        var company = CompanyBuilder.New()
            .WithCnpj("")
            .Build();

        var result = company.Validation();

        result.Should().BeFalse();
        company.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Company.Cnpj) && 
                                e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void ShouldAcceptNullFoundationDate()
    {
        var company = CompanyBuilder.New()
            .WithFoundationDate(null)
            .Build();

        var result = company.Validation();

        result.Should().BeTrue();
    }
    
    [Fact]
    public void ShouldAcceptFoundationDateAsToday()
    {
        var company = CompanyBuilder.New()
            .WithFoundationDate(DateTime.UtcNow)
            .Build();

        var result = company.Validation();

        result.Should().BeTrue();
        company.ValidationResult.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldFailWhenFoundationDateIsEqualTo19000101()
    {
        var company = CompanyBuilder.New()
            .WithFoundationDate(new DateTime(1900, 1, 1))
            .Build();

        var result = company.Validation();

        result.Should().BeFalse();
        company.ValidationResult.Errors
            .Should()
            .Contain(e => e.PropertyName == nameof(Company.FoundationDate));
    }

    [Fact]
    public void ShouldFailWhenFoundationDateIsInTheFuture()
    {
        var company = CompanyBuilder.New()
            .WithFoundationDate(DateTime.UtcNow.AddDays(1))
            .Build();

        var result = company.Validation();

        result.Should().BeFalse();
        company.ValidationResult.Errors
            .Should()
            .Contain(e => e.PropertyName == nameof(Company.FoundationDate) && e.ErrorMessage.Contains("future", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validation_ShouldBeIdempotent_WhenCalledTwice()
    {
        var company = CompanyBuilder.New()
            .WithName("DB")
            .WithCnpj("41.977.659/0001-800")
            .WithFoundationDate(DateTime.UtcNow.AddDays(1))
            .Build();

        var firstValid = company.Validation();
        var firstErrors = company.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        var secondValid = company.Validation();
        var secondErrors = company.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        firstValid.Should().BeFalse();
        secondValid.Should().BeFalse();
        secondErrors.Should().BeEquivalentTo(firstErrors);
    }
    
    [Fact]
    public void ShouldFailWhenFoundationDateIsBefore19000101()
    {
        var company = CompanyBuilder.New()
            .WithFoundationDate(new DateTime(1899, 12, 31))
            .Build();

        var result = company.Validation();
        
        result.Should().BeFalse();
        company.ValidationResult.Errors
            .Should()
            .ContainSingle(e =>
                e.PropertyName == nameof(Company.FoundationDate)
                && e.ErrorMessage.Contains("01/01/1900"));
    }
}
