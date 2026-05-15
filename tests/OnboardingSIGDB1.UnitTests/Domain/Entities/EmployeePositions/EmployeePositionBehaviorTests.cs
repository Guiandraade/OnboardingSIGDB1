using FluentAssertions;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Entities.EmployeePositions;

public class EmployeePositionBehaviorTests
{
    [Fact]
    public void Constructor_ShouldAssignEmployeeIdAndPositionIdWhenEntitiesHaveId()
    {
        var employee = EmployeeBuilder.New().Build();
        var position = PositionBuilder.New().Build();
        EmployeePositionBuilder.SetId(employee, 11);
        EmployeePositionBuilder.SetId(position, 22);

        var ep = new EmployeePosition(employee, position, DateTime.UtcNow.AddDays(-1));

        ep.EmployeeId.Should().Be(11);
        ep.PositionId.Should().Be(22);
    }

    [Fact]
    public void ShouldFailWhenStartDateIsExactly19000101()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(new DateTime(1900, 1, 1))
            .Build();

        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(EmployeePosition.StartDate)
                                                               && e.ErrorMessage.Contains("01/01/1900"));
    }

    [Fact]
    public void ShouldBeValidWhenStartDateIsOneDayAfter19000101()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(new DateTime(1900, 1, 2))
            .Build();

        var result = ep.Validation();

        result.Should().BeTrue();
        ep.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidWhenStartDateIsToday()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(DateTime.UtcNow.Date)
            .Build();

        var result = ep.Validation();

        result.Should().BeTrue();
        ep.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldBeValidWhenStartDateIsInThePast()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(DateTime.UtcNow.AddDays(-1))
            .Build();

        var result = ep.Validation();

        result.Should().BeTrue();
        ep.ValidationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ShouldFailWhenStartDateIsInTheFuture()
    {
        var ep = EmployeePositionBuilder.New()
            .WithEmployeeId(1)
            .WithPositionId(1)
            .WithStartDate(DateTime.UtcNow.AddDays(1))
            .Build();

        var result = ep.Validation();

        result.Should().BeFalse();
        ep.ValidationResult.Errors.Should().ContainSingle(e => e.PropertyName == nameof(EmployeePosition.StartDate));
    }
}