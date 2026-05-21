using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceSearchTests : EmployeeServiceTestBase
{
    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyAndNotify_WhenFilterIsInvalid()
    {
        var failures = new List<ValidationFailure> { new("PageNumber", "Page number must be greater than 0.") };
        _employeeFilterValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<EmployeeFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var service = CreateService();
        var result = await service.SearchAsync(new EmployeeFilter { PageNumber = 0, PageSize = 10 });
        result.Data.Should().BeEmpty();
        result.Total.Should().Be(0);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPagedResponse_WhenFilterIsValid()
    {
        var filter = new EmployeeFilter { PageNumber = 1, PageSize = 10 };
        var employees = new List<Employee> { EmployeeBuilder.New().Build() };
        var responses = new List<EmployeeResponse> { new() { Name = "Test" } };
        _employeeRepositoryMock.Setup(r => r.SearchAsync(filter)).ReturnsAsync((employees, 1));
        _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeResponse>>(employees)).Returns(responses);
        var service = CreateService();
        var result = await service.SearchAsync(filter);
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Total.Should().Be(1);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }
}

