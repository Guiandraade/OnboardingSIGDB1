using System.Reflection;
using FluentAssertions;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.Employees;

public class EmployeeBehaviorTests
{
    [Fact]
    public void Constructor_ShouldTreatNullCpfAsEmpty()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf(null!)
            .WithHireDate(null)
            .Build();
        
        employee.Cpf.Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldTreatNullCpfAsEmpty()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf(null!)
            .Build();

        employee.Update("NewName", null!);

        employee.Cpf.Should().BeEmpty();
    }
    
    [Fact]
    public void ShouldNormalizeCpfRemovingMask()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf("342.266.960-42")
            .WithHireDate(DateTime.Now.AddDays(-1))
            .Build();

        employee.Validation();

        employee.Cpf.Should().Be("34226696042");
    }
    
    [Fact]
    public void ShouldUpdatePropertiesWhenUpdateIsCalled()
    {
        var employee = EmployeeBuilder.New()
            .WithName("test")
            .WithCpf("342.266.960-42")
            .Build();

        employee.Update("Test Updated", "422.309.780-63");

        employee.Name.Should().Be("Test Updated");
        employee.Cpf.Should().Be("42230978063");
    }

    [Fact]
    public void GetLastPosition_ShouldReturnMostRecentPosition()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf("707.207.220-98")
            .WithHireDate(null)
            .Build();

        var pos1 = PositionBuilder.New().WithDescription("Developer").Build();
        var pos2 = PositionBuilder.New().WithDescription("Senior Developer").Build();

        var ep1 = new EmployeePosition(employee, pos1, new DateTime(2020, 1, 1));
        var ep2 = new EmployeePosition(employee, pos2, new DateTime(2021, 1, 1));

        AddPositionViaReflection(employee, ep1);
        AddPositionViaReflection(employee, ep2);

        var last = employee.GetLastPosition();

        last.Should().BeSameAs(ep2);
        employee.CurrentPositionDescription.Should().Be("Senior Developer");
    }

    [Fact]
    public void Positions_ShouldBeReadOnly()
    {
        var employee = EmployeeBuilder.New()
            .WithCpf("707.207.220-98")
            .WithHireDate(null)
            .Build();

        var positions = employee.Positions;

        ((IList<EmployeePosition>)positions).Invoking(l => l.Add(
                new EmployeePosition(employee, PositionBuilder.New().WithDescription("Dev").Build(), DateTime.UtcNow.AddDays(-1))))
            .Should().Throw<NotSupportedException>();
    }

    private static void AddPositionViaReflection(Employee employee, EmployeePosition ep)
    {
        var field = typeof(Employee).GetField("_positions", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var list = (List<EmployeePosition>)field.GetValue(employee)!;
        list.Add(ep);
    }
}