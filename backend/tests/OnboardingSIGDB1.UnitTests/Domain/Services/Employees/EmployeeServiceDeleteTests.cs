using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceDeleteTests : EmployeeServiceTestBase
{
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndNotify_WhenEmployeeNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification("Employee", It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _employeeRepositoryMock.Verify(r => r.Delete(It.IsAny<Employee>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAndReturnTrue_WhenValid()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        result.Should().BeTrue();
        _employeeRepositoryMock.Verify(r => r.Delete(employee), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndNotify_WhenCommitFails()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        result.Should().BeFalse();
        _employeeRepositoryMock.Verify(r => r.Delete(employee), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }
}

