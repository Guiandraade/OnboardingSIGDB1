using FluentAssertions;
using OnboardingSIGDB1.Domain.Dto.Companies.Responses;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Dto.Positions.Responses;

namespace OnboardingSIGDB1.UnitTests.Domain.Dto;

public class DtoDefaultValuesTests
{
    [Fact]
    public void CompanyResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new CompanyResponse();
        dto.Name.Should().BeEmpty();
        dto.Cnpj.Should().BeEmpty();
    }

    [Fact]
    public void CompanyAndEmployeesResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new CompanyAndEmployeesResponse();
        dto.Name.Should().BeEmpty();
        dto.Cnpj.Should().BeEmpty();
        dto.EmployeesPositionHistory.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void CompanyDetailsResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new CompanyDetailsResponse();
        dto.EmployeeName.Should().BeEmpty();
        dto.PositionName.Should().BeEmpty();
    }

    [Fact]
    public void EmployeeResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new EmployeeResponse();
        dto.Name.Should().BeEmpty();
        dto.Cpf.Should().BeEmpty();
        dto.CompanyName.Should().BeEmpty();
        dto.CurrentPosition.Should().BeEmpty();
    }

    [Fact]
    public void EmployeeAndPositionsResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new EmployeeAndPositionsResponse();
        dto.Name.Should().BeEmpty();
        dto.Cpf.Should().BeEmpty();
        dto.CompanyName.Should().BeEmpty();
        dto.CurrentPosition.Should().BeEmpty();
        dto.PositionHistory.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void EmployeePositionHistoryResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new EmployeePositionHistoryResponse();
        dto.PositionName.Should().BeEmpty();
    }

    [Fact]
    public void PositionResponse_DefaultValues_ShouldBeStringEmpty()
    {
        var dto = new PositionResponse();
        dto.Description.Should().BeEmpty();
    }
}

