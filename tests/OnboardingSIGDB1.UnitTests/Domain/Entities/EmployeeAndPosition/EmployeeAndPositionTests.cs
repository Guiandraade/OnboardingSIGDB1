using FluentAssertions;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.EmployeeAndPosition;

public class EmployeeAndPositionTests
{
    [Fact]
    public void Constructor_ShouldTreatEmployeeIdAsZeroWhenNull()
    {
        var employeeAndPosition = new EmployeePosition(null!, null!, DateTime.UtcNow);
        employeeAndPosition.EmployeeId.Should().Be(0);
    }

    [Fact]
    public void Constructor_ShouldTreatPositionIdAsZeroWhenNull()
    {
        var employeeAndPosition = new EmployeePosition(null!, null!, DateTime.UtcNow);
        employeeAndPosition.PositionId.Should().Be(0);
    }

    [Fact]
    public void ShouldFailWhenStartDateIsDefault()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(default)
            .Build();

        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should()
            .ContainSingle(e => e.PropertyName == nameof(EmployeePosition.StartDate)
                                && e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void ShouldFillEndDateWhenClosingPosition()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(DateTime.UtcNow.AddDays(-1))
            .Build();

        ep.ClosePosition(DateTime.UtcNow);
        var result = ep.Validation();

        result.Should().BeTrue();
        ep.EndDate.Should().NotBeNull();
    }

    [Fact]
    public void Validation_ShouldFailWhenEmployeeIsNull()
    {
        var position = PositionBuilder.New().Build();
        EmployeePositionBuilder.SetId(position, 1);

        var ep = new EmployeePosition(null!, position, DateTime.UtcNow.AddDays(-1));
        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should().Contain(e => e.PropertyName == nameof(EmployeePosition.Employee));
    }

    [Fact]
    public void Validation_ShouldFailWhenPositionIsNull()
    {
        var employee = EmployeeBuilder.New().Build();
        EmployeePositionBuilder.SetId(employee, 1);

        var ep = new EmployeePosition(employee, null!, DateTime.UtcNow.AddDays(-1));
        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should().Contain(e => e.PropertyName == nameof(EmployeePosition.Position));
    }

    [Fact]
    public void Validation_ShouldBeIdempotent_WhenCalledTwice()
    {
        var ep = new EmployeePosition(null!, null!, default);

        var firstValid = ep.Validation();
        var firstErrors = ep.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        var secondValid = ep.Validation();
        var secondErrors = ep.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage)).ToList();

        firstValid.Should().BeFalse();
        secondValid.Should().BeFalse();
        secondErrors.Should().BeEquivalentTo(firstErrors);
    }

    [Fact]
    public void Validation_ShouldFailWhenPositionIdIsZero()
    {
        var employee = EmployeeBuilder.New().Build();
        EmployeePositionBuilder.SetId(employee, 1);
        var position = PositionBuilder.New().Build(); // Id == 0

        var ep = new EmployeePosition(employee, position, DateTime.UtcNow.AddDays(-1));
        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should().Contain(e => e.PropertyName == nameof(EmployeePosition.PositionId));
    }

    [Fact]
    public void ShouldBeValidWhenEndDateEqualsStartDate()
    {
        var start = DateTime.UtcNow.AddDays(-1);
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(start)
            .Build();

        ep.ClosePosition(start);
        var result = ep.Validation();

        result.Should().BeTrue();
        ep.ValidationResult.Errors.Should().NotContain(e => e.PropertyName == nameof(EmployeePosition.EndDate));
    }

    [Fact]
    public void ShouldFailWhenEndDateIsBeforeStartDate()
    {
        var start = DateTime.UtcNow.AddDays(-1);
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(start)
            .Build();

        ep.ClosePosition(start.AddSeconds(-1));
        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should().Contain(e => e.PropertyName == nameof(EmployeePosition.EndDate));
    }
}