using FluentAssertions;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceGetTests : EmployeeServiceTestBase
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullAndNotify_WhenNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("Employee", It.Is<string>(s => s.Contains("not found"))), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnResponse_WhenFound()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _mapperMock.Setup(m => m.Map<EmployeeResponse>(employee)).Returns(new EmployeeResponse { Id = 1 });
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mapperMock.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task GetHistoryAsync_ShouldReturnNullAndNotify_WhenNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetHistoryAsync(1)).ReturnsAsync((Employee?)null);
        var service = CreateService();
        var result = await service.GetHistoryAsync(1);
        result.Should().BeNull();
        _mapperMock.Verify(m => m.Map<EmployeeAndPositionsResponse>(It.IsAny<Employee>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddNotification("Employee", It.Is<string>(s => s.Contains("not found"))), Times.Once);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldReturnResponse_WhenFound()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetHistoryAsync(1)).ReturnsAsync(employee);
        _mapperMock.Setup(m => m.Map<EmployeeAndPositionsResponse>(employee)).Returns(new EmployeeAndPositionsResponse { Id = 1 });
        var service = CreateService();
        var result = await service.GetHistoryAsync(1);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mapperMock.Verify(m => m.Map<EmployeeAndPositionsResponse>(It.IsAny<Employee>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

