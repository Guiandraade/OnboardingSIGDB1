using FluentAssertions;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
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

    [Fact]
    public void Records_ShouldSupportEqualityAndToString()
    {
        var company = new CompanyResponse { Id = 1, Name = "ACME", Cnpj = "123" };
        var companyCopy = new CompanyResponse { Id = 1, Name = "ACME", Cnpj = "123" };
        var employee = new EmployeeResponse { Id = 7, Name = "John", Cpf = "123" };
        var employeeCopy = new EmployeeResponse { Id = 7, Name = "John", Cpf = "123" };
        var companyWithEmployees = new CompanyAndEmployeesResponse { Id = 1, Name = "ACME", Cnpj = "123" };
        var companyWithEmployeesCopy = new CompanyAndEmployeesResponse { Id = 1, Name = "ACME", Cnpj = "123" };
        var companyDetails = new CompanyDetailsResponse { EmployeeId = 5, EmployeeName = "Mary", PositionName = "Dev" };
        var companyDetailsCopy = new CompanyDetailsResponse { EmployeeId = 5, EmployeeName = "Mary", PositionName = "Dev" };
        var employeeAndPositions = new EmployeeAndPositionsResponse { Id = 3, Name = "Jane", Cpf = "321" };
        var employeeAndPositionsCopy = new EmployeeAndPositionsResponse { Id = 3, Name = "Jane", Cpf = "321" };
        var position = new PositionResponse { Id = 9, Description = "Tech Lead" };
        var positionCopy = new PositionResponse { Id = 9, Description = "Tech Lead" };
        var companyClone = company with { };
        var employeeClone = employee with { };
        var companyWithEmployeesClone = companyWithEmployees with { };
        var companyDetailsClone = companyDetails with { };
        var employeeAndPositionsClone = employeeAndPositions with { };
        var positionClone = position with { };

        company.Should().Be(companyCopy);
        employee.Should().Be(employeeCopy);
        companyWithEmployees.Should().BeEquivalentTo(companyWithEmployeesCopy);
        companyDetails.Should().Be(companyDetailsCopy);
        employeeAndPositions.Should().BeEquivalentTo(employeeAndPositionsCopy);
        position.Should().Be(positionCopy);
        company.ToString().Should().Contain("CompanyResponse");
        employee.ToString().Should().Contain("EmployeeResponse");
        companyWithEmployees.ToString().Should().Contain("CompanyAndEmployeesResponse");
        companyDetails.ToString().Should().Contain("CompanyDetailsResponse");
        employeeAndPositions.ToString().Should().Contain("EmployeeAndPositionsResponse");
        position.ToString().Should().Contain("PositionResponse");
        companyClone.Should().Be(company);
        employeeClone.Should().Be(employee);
        companyWithEmployeesClone.Should().BeEquivalentTo(companyWithEmployees);
        companyDetailsClone.Should().Be(companyDetails);
        employeeAndPositionsClone.Should().BeEquivalentTo(employeeAndPositions);
        positionClone.Should().Be(position);
    }

    [Fact]
    public void Filters_ShouldStoreStringProperties()
    {
        var companyFilter = new CompanyFilter { Name = "ACME", Cnpj = "12.345.678/0001-90" };
        var employeeFilter = new EmployeeFilter { Name = "John", Cpf = "123.456.789-01" };

        companyFilter.Name.Should().Be("ACME");
        companyFilter.Cnpj.Should().Be("12.345.678/0001-90");
        employeeFilter.Name.Should().Be("John");
        employeeFilter.Cpf.Should().Be("123.456.789-01");
    }
}

