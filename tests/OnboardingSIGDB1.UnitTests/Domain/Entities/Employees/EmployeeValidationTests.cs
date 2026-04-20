using FluentAssertions;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.Employees;

public class EmployeeValidationTests
{
    [Fact]
    public void ShouldBeValidWhenAllPropertiesAreValid()
    {
        var employee = EmployeeBuilder.New().Build();
        
        var result = employee.Validation();

        result.Should().BeTrue();
        employee.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldContainFourValidationErrors()
    {
        var employee = EmployeeBuilder.New()
            .WithName("Gu")
            .WithCpf("707.207.220-904")
            .WithHireDate(DateTime.UtcNow.AddDays(1))
            .WithCompanyId(0)
            .Build();

        var result = employee.Validation();
        result.Should().BeFalse();

        var propertiesWithErrors = employee.ValidationResult.Errors
            .Select(e => e.PropertyName)
            .ToList();

        propertiesWithErrors.Should().Contain(new[]
        {
            nameof(Employee.Name),
            nameof(Employee.Cpf),
            nameof(Employee.HireDate)
        });
    }

    [Fact]
    public void ShouldBeValidWhenHireDateIsNull()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(null)
            .Build();

        var result = employee.Validation();

        result.Should().BeTrue();
        employee.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidWhenHireDateIsToday()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeTrue();
        employee.ValidationResult.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldFailWhenHireDateIsInTheFuture()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(DateTime.UtcNow.AddDays(1))
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .Contain(e => e.PropertyName == nameof(Employee.HireDate));
    }
    
    [Fact]
    public void ShouldFailWhenHireDateIsBefore19000101()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(new DateTime(1899, 12, 31))
            .Build();

        var result = employee.Validation();
        
        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e =>
                e.PropertyName == nameof(Employee.HireDate)
                && e.ErrorMessage.Contains("01/01/1900"));
    }
    
    [Fact]
    public void ShouldFailWhenHireDateIsExactly19000101()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(new DateTime(1900, 1, 1))
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.HireDate) && e.ErrorMessage.Contains("01/01/1900"));
    }

    [Fact]
    public void ShouldBeValidWhenHireDateIsOneDayAfter19000101()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(new DateTime(1900, 1, 2))
            .Build();

        var result = employee.Validation();

        result.Should().BeTrue();
        employee.ValidationResult.Errors.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldBeValidWhenHireDateIsInThePast()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(DateTime.UtcNow.AddDays(-1))
            .Build();

        var result = employee.Validation();
        result.Should().BeTrue();
        employee.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldFailWhenHireDateIsTomorrow()
    {
        var employee = EmployeeBuilder.New()
            .WithHireDate(DateTime.UtcNow.AddDays(1))
            .Build();

        var result = employee.Validation();
        result.Should().BeFalse();
        employee.ValidationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(Employee.HireDate));
    }

    [Theory]
    [InlineData("Gu")] 
    [InlineData("")] 
    public void ShouldFailWhenNameIsLessThanThreeOrEmpty(string name)
    {
        var employee = EmployeeBuilder.New()
            .WithName(name)
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .Contain(e => e.PropertyName == nameof(Employee.Name));
    }
    
    [Fact]
    public void ShouldAcceptNameWithExactlyThreeCharacters()
    {
        var employee = EmployeeBuilder.New()
            .WithName("abc")
            .WithHireDate(DateTime.UtcNow)
            .Build();

        employee.Validation().Should().BeTrue();
    }
    
    [Fact]
    public void ShouldFailWhenCpfIsNull()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf(null!)
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.Cpf) &&
                                e.ErrorMessage.Contains("required"));
    }
    
    [Theory]
    [InlineData("1111111111")]    
    [InlineData("111111111111")]  
    [InlineData("111")]           
    public void ShouldFailWhenCpfIsNot11Digits(string cpf)
    {
        var employee = EmployeeBuilder.New()
            .WithCpf(cpf)
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.Cpf) &&
                                e.ErrorMessage.Contains("11"));
    }
    
    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("22222222222")] 
    public void ShouldFailWhenCpfIsInvalid(string cpf)
    {
        var employee = EmployeeBuilder.New()
            .WithCpf(cpf)
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.Cpf) &&
                                e.ErrorMessage.Contains("invalid"));
    }
    
    [Fact]
    public void ShouldFailWhenCompanyIdIsZero()
    {
        var employee = EmployeeBuilder.New()
            .WithCompanyId(0)
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.CompanyId));
    }

    [Fact]
    public void ShouldFailWhenCompanyIdIsNegative()
    {
        var employee = EmployeeBuilder.New()
            .WithCompanyId(-1)
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();

        result.Should().BeFalse();
        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.CompanyId));
    }

    [Fact]
    public void ShouldFailWhenNameExceeds150Characters()
    {
        var employee = EmployeeBuilder.New()
            .WithName(new string('A', 151))
            .WithHireDate(DateTime.UtcNow)
            .Build();

        var result = employee.Validation();
        result.Should().BeFalse();

        employee.ValidationResult.Errors
            .Should()
            .ContainSingle(e => e.PropertyName == nameof(Employee.Name));
    }

    [Fact]
    public void Validation_ShouldBeIdempotent_WhenCalledTwice()
    {
        var employee = EmployeeBuilder.New()
            .WithName("Gu")
            .WithCpf("707.207.220-904")
            .WithHireDate(DateTime.UtcNow.AddDays(1))
            .WithCompanyId(0)
            .Build();

        var firstValid = employee.Validation();
        var firstErrors = employee.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        var secondValid = employee.Validation();
        var secondErrors = employee.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        firstValid.Should().BeFalse();
        secondValid.Should().BeFalse();
        secondErrors.Should().BeEquivalentTo(firstErrors);
    }

    [Fact]
    public void CurrentPositionDescription_ShouldReturnNoLink_WhenNoPositions()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf("707.207.220-98")
            .WithHireDate(null)
            .Build();

        employee.CurrentPositionDescription.Should().Be("No link");
        employee.GetLastPosition().Should().BeNull();
    }
}
