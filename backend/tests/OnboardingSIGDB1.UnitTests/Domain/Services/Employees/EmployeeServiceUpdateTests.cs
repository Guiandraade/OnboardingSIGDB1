using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceUpdateTests : EmployeeServiceTestBase
{
    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndNotify_WhenEmployeeNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);
        var service = CreateService();
        var result = await service.UpdateAsync(1, new EmployeeUpdateRequest("Test", "987.826.470-03"));
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("Employee", It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndNotify_WhenCpfConflictExists()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        var other = EmployeeBuilder.New().WithId(2).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync(other);
        var service = CreateService();
        var result = await service.UpdateAsync(1, new EmployeeUpdateRequest("Test", "987.826.470-03"));
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("Cpf", It.Is<string>(s => s.Contains("already in use"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndNotify_WhenDomainValidationFails()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        var service = CreateService();
        var result = await service.UpdateAsync(1, new EmployeeUpdateRequest("", "987.826.470-03"));
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndReturnResponse_WhenValid()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<EmployeeResponse>(It.IsAny<Employee>())).Returns(new EmployeeResponse { Name = "Updated" });
        var service = CreateService();
        var result = await service.UpdateAsync(1, new EmployeeUpdateRequest("Updated", "987.826.470-03"));
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated");
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _employeeRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var result = await service.UpdateAsync(1, new EmployeeUpdateRequest("Updated", "987.826.470-03"));
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _mapperMock.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }
}

