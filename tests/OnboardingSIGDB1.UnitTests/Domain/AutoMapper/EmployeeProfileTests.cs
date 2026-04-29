using AutoMapper;
using FluentAssertions;
using OnboardingSIGDB1.Domain.AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using System.Reflection;

namespace OnboardingSIGDB1.UnitTests.Domain.AutoMapper;

public class EmployeeProfileTests : MapperTestBase
{
    private readonly IMapper _mapper;

    public EmployeeProfileTests()
    {
        _mapper = CreateMapper(new EmployeeProfile());
    }
    
    [Fact]
    public void EmployeeRequest_To_Employee_MapsNameCpfHireDate()
    {
        var request = new EmployeeRequest("Test", "12345678901", new DateTime(2023, 1, 15), 1, 10);

        var entity = _mapper.Map<Employee>(request);

        entity.Should().NotBeNull();
        entity.Name.Should().Be("Test");
        entity.Cpf.Should().Be("12345678901");
        entity.HireDate.Should().Be(new DateTime(2023, 1, 15));
    }

    [Fact]
    public void EmployeeRequest_To_Employee_IgnoresIdCompanyPositionsValidation()
    {
        var request = new EmployeeRequest("Test", "12345678901", null, 5, 10);

        var entity = _mapper.Map<Employee>(request);

        entity.Id.Should().Be(0);
        entity.Company.Should().BeNull();
        entity.Positions.Should().BeEmpty();
        entity.ValidationResult.Should().BeNull();
    }

    [Fact]
    public void EmployeeRequest_To_Employee_NullHireDate()
    {
        var request = new EmployeeRequest("Test", "12345678901", null, 1, 1);

        var entity = _mapper.Map<Employee>(request);

        entity.HireDate.Should().BeNull();
    }
    
    [Fact]
    public void EmployeeUpdateRequest_To_Employee_MapsNameAndCpf()
    {
        var request = new EmployeeUpdateRequest("Test", "98765432100");

        var entity = _mapper.Map<Employee>(request);

        entity.Should().NotBeNull();
        entity.Name.Should().Be("Test");
        entity.Cpf.Should().Be("98765432100");
    }

    [Fact]
    public void EmployeeUpdateRequest_To_Employee_IgnoresIdHireDateCompanyPositions()
    {
        var request = new EmployeeUpdateRequest("Test", "12345678901");

        var entity = _mapper.Map<Employee>(request);

        entity.Id.Should().Be(0);
        entity.HireDate.Should().BeNull();
        entity.CompanyId.Should().Be(0);
        entity.Company.Should().BeNull();
        entity.Positions.Should().BeEmpty();
        entity.ValidationResult.Should().BeNull();
    }

    [Fact]
    public void Employee_To_EmployeeResponse_MapsAllProperties()
    {
        var company = new Company("Test Company", "41977659000180", null);
        var employee = new Employee("Test Employee", "12345678901", new DateTime(2023, 1, 1), 1);

        SetProperty(employee, nameof(Employee.Company), company);

        var dto = _mapper.Map<EmployeeResponse>(employee);

        dto.Should().NotBeNull();
        dto.Id.Should().Be(0);
        dto.Name.Should().Be("Test Employee");
        dto.Cpf.Should().Be("12345678901");
        dto.HireDate.Should().Be(new DateTime(2023, 1, 1));
        dto.CompanyName.Should().Be("Test Company");
        dto.CurrentPosition.Should().Be("No link");
    }

    [Fact]
    public void Employee_To_EmployeeResponse_WithPosition_MapsCurrentPosition()
    {
        var company = new Company("ACME", "41977659000180", null);
        var employee = new Employee("João", "12345678901", new DateTime(2023, 1, 1), 1);
        var position = new Position("Developer");

        SetProperty(employee, nameof(Employee.Company), company);
        AddToPrivateList<Employee, EmployeePosition>(employee, "_positions",
            new EmployeePosition(employee, position, DateTime.UtcNow));

        var dto = _mapper.Map<EmployeeResponse>(employee);

        dto.CurrentPosition.Should().Be("Developer");
    }

    [Fact]
    public void Employee_To_EmployeeAndPositionsResponse_MapsBasicProperties()
    {
        var company = new Company("ACME", "41977659000180", null);
        var employee = new Employee("Maria", "98765432100", new DateTime(2022, 6, 1), 1);
        SetProperty(employee, nameof(Employee.Company), company);

        var dto = _mapper.Map<EmployeeAndPositionsResponse>(employee);

        dto.Should().NotBeNull();
        dto.Name.Should().Be("Maria");
        dto.Cpf.Should().Be("98765432100");
        dto.HireDate.Should().Be(new DateTime(2022, 6, 1));
        dto.CompanyName.Should().Be("ACME");
        dto.CurrentPosition.Should().Be("No link");
        dto.PositionHistory.Should().BeEmpty();
    }

    [Fact]
    public void Employee_To_EmployeeAndPositionsResponse_WithPositions_MapsOrderedByStartDateDesc()
    {
        var company = new Company("ACME", "41977659000180", null);
        var employee = new Employee("Maria", "98765432100", new DateTime(2022, 6, 1), 1);
        var positionA = new Position("Junior");
        var positionB = new Position("Senior");

        SetProperty(employee, nameof(Employee.Company), company);
        var positions = GetPrivateList<Employee, EmployeePosition>(employee, "_positions");
        positions.Add(new EmployeePosition(employee, positionA, new DateTime(2022, 1, 1)));
        positions.Add(new EmployeePosition(employee, positionB, new DateTime(2023, 6, 1)));

        var dto = _mapper.Map<EmployeeAndPositionsResponse>(employee);

        dto.PositionHistory.Should().HaveCount(2);
        dto.PositionHistory[0].PositionName.Should().Be("Senior");
        dto.PositionHistory[0].StartDate.Should().Be(new DateTime(2023, 6, 1));
        dto.PositionHistory[1].PositionName.Should().Be("Junior");
        dto.PositionHistory[1].StartDate.Should().Be(new DateTime(2022, 1, 1));
    }

    [Fact]
    public void EmployeePosition_To_EmployeePositionHistoryResponse_MapsPositionNameAndStartDate()
    {
        var employee = new Employee("Test", "12345678901", DateTime.UtcNow, 1);
        var position = new Position("Architect");
        var startDate = new DateTime(2024, 3, 15);
        var ep = new EmployeePosition(employee, position, startDate);

        var dto = _mapper.Map<EmployeePositionHistoryResponse>(ep);

        dto.Should().NotBeNull();
        dto.PositionName.Should().Be("Architect");
        dto.StartDate.Should().Be(startDate);
    }

    private static void SetProperty<T>(T obj, string propertyName, object? value)
    {
        typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(obj, value);
    }

    private static void AddToPrivateList<TOwner, TItem>(TOwner obj, string fieldName, TItem item)
    {
        var list = GetPrivateList<TOwner, TItem>(obj, fieldName);
        list.Add(item);
    }

    private static List<TItem> GetPrivateList<TOwner, TItem>(TOwner obj, string fieldName)
    {
        var field = typeof(TOwner).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (List<TItem>)field.GetValue(obj)!;
    }
}


