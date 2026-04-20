using FluentAssertions;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.Companies;

public class CompanyBehaviorTests
{
    
    [Fact]
    public void Constructor_ShouldTreatNullCnpjAsEmpty()
    {
        var company = CompanyBuilder.New()
            .WithCnpj(null!)
            .Build();
        
        company.Cnpj.Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldTreatNullCnpjAsEmpty()
    {
        var company = CompanyBuilder.New().Build();
        
        company.Update("NewName", null!, null);

        company.Cnpj.Should().BeEmpty();
        company.Name.Should().Be("NewName");
        company.FoundationDate.Should().BeNull();
    }
    
    [Theory]
    [InlineData("41.977.659/0001-80", "41977659000180")]
    [InlineData("41977659000180", "41977659000180")]
    [InlineData(" 41.977.659/0001-80 ", "41977659000180")]
    public void ShouldNormalizeCnpjInConstructor(string input, string expected)
    {
        var company = CompanyBuilder.New()
            .WithCnpj(input)
            .Build();
        
        company.Cnpj.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("41.977.659/0001-80", "41977659000180")]
    [InlineData("41977659000180", "41977659000180")]
    [InlineData(" 41.977.659/0001-80 ", "41977659000180")]
    public void ShouldNormalizeCnpjInUpdate(string input, string expected)
    {
        var company = CompanyBuilder.New().Build();
        
        company.Update("NewName", input, null);
        
        company.Cnpj.Should().Be(expected);
    }

    [Fact]
    public void ShouldUpdatePropertiesWhenUpdateIsCalled()
    {
        var company = new CompanyBuilder().Build();
        
        company.Update("NewName", "41.977.659/0001-80", new DateTime(2000, 4, 16));
        
        company.Name.Should().Be("NewName");
        company.Cnpj.Should().Be("41977659000180");
        company.FoundationDate.Should().Be(new DateTime(2000, 4, 16));
    }
    
    [Fact]
    public void Employees_ShouldBeReadOnly()
    { 
        var company = CompanyBuilder.New().Build();

        var employees = company.Employees;
        
        employees.Should().BeAssignableTo<IReadOnlyCollection<Employee>>();

        ((IList<Employee>)employees).Invoking(l => l.Add(
                EmployeeBuilder.New().Build()))
            .Should().Throw<NotSupportedException>();
    }
}