using AutoMapper;
using FluentAssertions;
using OnboardingSIGDB1.Domain.AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Companies.Commands;
using OnboardingSIGDB1.Domain.Dto.Companies.Responses;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using System.Reflection;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.AutoMapper;

public class CompanyProfileTests : MapperTestBase
{
    private readonly IMapper _mapper;

    public CompanyProfileTests()
    {
        _mapper = CreateMapper(new CompanyProfile());
    }
    
    [Fact]
    public void Company_To_CompanyResponse_MapsAllProperties()
    {
        var entity = CompanyBuilder.New()
            .WithId(1)
            .WithName("Test")
            .WithCnpj("41977659000180")
            .WithFoundationDate(new DateTime(2020, 1, 1))
            .Build();

        var dto = _mapper.Map<CompanyResponse>(entity);

        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Test");
        dto.Cnpj.Should().Be("41977659000180");
        dto.FoundationDate.Should().Be(new DateTime(2020, 1, 1));
    }

    [Fact]
    public void Company_To_CompanyResponse_NullFoundationDate()
    {
        var entity = new Company("Test", "41977659000180", null);

        var dto = _mapper.Map<CompanyResponse>(entity);

        dto.FoundationDate.Should().BeNull();
    }

    [Fact]
    public void CompanyCollection_MapsTo_CompanyResponseCollection()
    {
        var list = new[]
        {
            CompanyBuilder.New().WithName("A").WithCnpj("11222333000181").WithFoundationDate(null).Build(),
            CompanyBuilder.New().WithName("B").WithCnpj("27284997000105").WithFoundationDate(null).Build()
        };

        var mapped = _mapper.Map<IEnumerable<CompanyResponse>>(list).ToList();

        mapped.Should().HaveCount(2);
        mapped[0].Name.Should().Be("A");
        mapped[0].Cnpj.Should().Be("11222333000181");
        mapped[0].FoundationDate.Should().BeNull();
        mapped[1].Name.Should().Be("B");
        mapped[1].Cnpj.Should().Be("27284997000105");
        mapped[1].FoundationDate.Should().BeNull();
    }

    [Fact]
    public void NullCompany_ReturnsNull()
    {
        Company? src = null;
        var dto = _mapper.Map<CompanyResponse?>(src);
        dto.Should().BeNull();
    }
    
    [Fact]
    public void CompanyRequest_To_Company_MapsNameCnpjFoundationDate()
    {
        var request = new CompanyRequest("New company", "11222333000181", new DateTime(2022, 6, 15));

        var entity = _mapper.Map<Company>(request);

        entity.Should().NotBeNull();
        entity.Name.Should().Be("New company");
        entity.Cnpj.Should().Be("11222333000181");
        entity.FoundationDate.Should().Be(new DateTime(2022, 6, 15));
    }

    [Fact]
    public void CompanyRequest_To_Company_IgnoresIdAndValidationResult()
    {
        var request = new CompanyRequest("Test", "11222333000181", null);

        var entity = _mapper.Map<Company>(request);

        entity.Id.Should().Be(0);
        entity.ValidationResult.Should().BeNull();
    }

    [Fact]
    public void CompanyRequest_To_Company_NullFoundationDate()
    {
        var request = new CompanyRequest("Test", "11222333000181", null);

        var entity = _mapper.Map<Company>(request);

        entity.FoundationDate.Should().BeNull();
    }
    
    [Fact]
    public void Company_To_CompanyAndEmployeesResponse_MapsBasicProperties()
    {
        var company = CompanyBuilder.New()
            .WithName("Test")
            .WithCnpj("41977659000180")
            .WithFoundationDate(new DateTime(2020, 1, 1))
            .Build();

        var dto = _mapper.Map<CompanyAndEmployeesResponse>(company);

        dto.Should().NotBeNull();
        dto.Name.Should().Be("Test");
        dto.Cnpj.Should().Be("41977659000180");
        dto.FoundationDate.Should().Be(new DateTime(2020, 1, 1));
        dto.EmployeesPositionHistory.Should().BeEmpty();
    }

    [Fact]
    public void Company_To_CompanyAndEmployeesResponse_MapsEmployeesToPositionHistory()
    {
        var company = new Company("Test Company", "41977659000180", null);
        var employee = new Employee("Test Employee", "12345678901", new DateTime(2023, 1, 1), 1);

        var employeesField = typeof(Company).GetField("_employees", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var employees = (List<Employee>)employeesField.GetValue(company)!;
        employees.Add(employee);

        var dto = _mapper.Map<CompanyAndEmployeesResponse>(company);

        dto.EmployeesPositionHistory.Should().HaveCount(1);
        dto.EmployeesPositionHistory[0].EmployeeName.Should().Be("Test Employee");
    }
    
    [Fact]
    public void Employee_To_CompanyDetailsResponse_MapsAllProperties()
    {
        var employee = new Employee("Test", "12345678901", new DateTime(2023, 3, 10), 1);
        var position = new Position("Developer");

        // Add a position via reflection to test CurrentPositionDescription
        var positionsField = typeof(Employee).GetField("_positions", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var positions = (List<EmployeePosition>)positionsField.GetValue(employee)!;
        positions.Add(new EmployeePosition(employee, position, DateTime.UtcNow));

        var dto = _mapper.Map<CompanyDetailsResponse>(employee);

        dto.EmployeeId.Should().Be(0); 
        dto.EmployeeName.Should().Be("Test");
        dto.PositionName.Should().Be("Developer");
        dto.HiringDate.Should().Be(new DateTime(2023, 3, 10));
    }

    [Fact]
    public void Employee_To_CompanyDetailsResponse_NoPosition_MapsNoLink()
    {
        var employee = new Employee("Test", "12345678901", new DateTime(2023, 5, 1), 1);

        var dto = _mapper.Map<CompanyDetailsResponse>(employee);

        dto.EmployeeName.Should().Be("Test");
        dto.PositionName.Should().Be("No link");
    }
}