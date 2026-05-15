using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceChangePositionTests : EmployeeServiceTestBase
{
    [Fact]
    public async Task ChangePositionAsync_ShouldReturnFalseAndNotify_WhenEmployeeNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync((Employee?)null);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(1));
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification("Employee", It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Never);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldReturnFalseAndNotify_WhenPositionNotFound()
    {
        var employee = EmployeeBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Position?)null);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(99));
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification("Position", It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Never);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldReturnFalseAndNotify_WhenEmployeeAlreadyHeldPosition()
    {
        var company = CompanyBuilder.New().Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompany(company).Build();
        var position = PositionBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _employeePositionsRepositoryMock.Setup(r => r.HasEmployeeEverHeldPosition(1, 1)).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(1));
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification("Position", It.Is<string>(s => s.Contains("already held"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldCloseActivePositionAndAssignNew_WhenEmployeeHasActivePosition()
    {
        var company = CompanyBuilder.New().Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompany(company).Build();
        var oldPosition = PositionBuilder.New().WithId(1).Build();
        var newPosition = PositionBuilder.New().WithId(2).Build();
        var activeEmployeePosition = EmployeePositionBuilder.New().WithEmployee(employee).WithPosition(oldPosition).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(newPosition);
        _employeePositionsRepositoryMock.Setup(r => r.HasEmployeeEverHeldPosition(1, 2)).ReturnsAsync(false);
        _employeePositionsRepositoryMock.Setup(r => r.GetActivePositionAsync(1)).ReturnsAsync(activeEmployeePosition);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(2));
        result.Should().BeTrue();
        activeEmployeePosition.EndDate.Should().NotBeNull();
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task ChangePositionAsync_ShouldReturnNullAndNotify_WhenDomainValidationFails()
    {
        var company = CompanyBuilder.New().Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompany(company).Build();
        var newPosition = PositionBuilder.New().WithId(2).Build();
        var activeEmployeePosition = EmployeePositionBuilder.New().WithEmployee(null!).WithPosition(null!).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(newPosition);
        _employeePositionsRepositoryMock.Setup(r => r.HasEmployeeEverHeldPosition(1, 2)).ReturnsAsync(false);
        _employeePositionsRepositoryMock.Setup(r => r.GetActivePositionAsync(1)).ReturnsAsync(activeEmployeePosition);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(2));
        result.Should().BeFalse();
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldReturnFalseAndNotify_WhenStartDateIsBeforeCompanyFoundationDate()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(DateTime.UtcNow.AddYears(10)).Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompanyId(1).WithCompany(company).Build();
        var position = PositionBuilder.New().WithId(2).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(position);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(2));
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification("StartDate", It.Is<string>(s => s.Contains("foundation date"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Never);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldProceed_WhenDateOfChangeEqualsFoundationDate()
    {
        var fixedDate = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc);
        _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(fixedDate);
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(fixedDate).Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompanyId(1).WithCompany(company).Build();
        var position = PositionBuilder.New().WithId(2).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(position);
        _employeePositionsRepositoryMock.Setup(r => r.HasEmployeeEverHeldPosition(1, 2)).ReturnsAsync(false);
        _employeePositionsRepositoryMock.Setup(r => r.GetActivePositionAsync(1)).ReturnsAsync((EmployeePosition?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(2));
        result.Should().BeTrue();
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.Is<EmployeePosition>(e => e.StartDate == fixedDate)), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldChangePositionAndReturnTrue_WhenValid()
    {
        var company = CompanyBuilder.New().Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompany(company).Build();
        var position = PositionBuilder.New().WithId(2).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(position);
        _employeePositionsRepositoryMock.Setup(r => r.HasEmployeeEverHeldPosition(1, 2)).ReturnsAsync(false);
        _employeePositionsRepositoryMock.Setup(r => r.GetActivePositionAsync(1)).ReturnsAsync((EmployeePosition?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(2));
        result.Should().BeTrue();
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePositionAsync_ShouldReturnFalseAndNotify_WhenCommitFails()
    {
        var company = CompanyBuilder.New().Build();
        var employee = EmployeeBuilder.New().WithId(1).WithCompany(company).Build();
        var position = PositionBuilder.New().WithId(2).Build();
        _employeeRepositoryMock.Setup(r => r.GetByIdWithCompanyAsync(1)).ReturnsAsync(employee);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(position);
        _employeePositionsRepositoryMock.Setup(r => r.HasEmployeeEverHeldPosition(1, 2)).ReturnsAsync(false);
        _employeePositionsRepositoryMock.Setup(r => r.GetActivePositionAsync(1)).ReturnsAsync((EmployeePosition?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var result = await service.ChangePositionAsync(1, new ChangeEmployeePositionRequest(2));
        result.Should().BeFalse();
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }
}

