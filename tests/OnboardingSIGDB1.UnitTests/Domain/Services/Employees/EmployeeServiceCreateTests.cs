using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceCreateTests : EmployeeServiceTestBase
{
    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCpfAlreadyExists()
    {
        var existing = EmployeeBuilder.New().Build();
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync(existing);
        var service = CreateService();
        var request = new EmployeeRequest("Test", "987.826.470-03", DateTime.UtcNow.AddDays(-1), 1, 1);
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("Cpf", It.Is<string>(s => s.Contains("already registered"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCompanyNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((OnboardingSIGDB1.Domain.Entities.Companies.Company?)null);
        var service = CreateService();
        var request = new EmployeeRequest("Test", "987.826.470-03", null, 99, 1);
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("Company", It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        _mapperMock.Verify(r => r.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenPositionNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CompanyBuilder.New().WithId(1).Build());
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Position?)null);
        var service = CreateService();
        var request = new EmployeeRequest("Test", "987.826.470-03", null, 1, 99);
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("Position", It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        _mapperMock.Verify(r => r.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenHireDateIsBeforeFoundationDate()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2020, 1, 1)).Build();
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PositionBuilder.New().WithId(1).Build());
        var service = CreateService();
        var request = new EmployeeRequest("Test", "987.826.470-03", new DateTime(2019, 6, 1), 1, 1);
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification("HireDate", It.Is<string>(s => s.Contains("earlier"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        _mapperMock.Verify(r => r.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenDomainValidationFails()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2000, 1, 1)).Build();
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PositionBuilder.New().WithId(1).Build());
        var service = CreateService();
        var request = new EmployeeRequest("", "987.826.470-03", new DateTime(2020, 1, 1), 1, 1);
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnResponse_WhenValid()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2000, 1, 1)).Build();
        var position = PositionBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<EmployeeResponse>(It.IsAny<Employee>())).Returns(new EmployeeResponse { Name = "John Doe" });
        var service = CreateService();
        var request = new EmployeeRequest("John Doe", "987.826.470-03", new DateTime(2020, 1, 1), 1, 1);
        var result = await service.CreateAsync(request);
        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2000, 1, 1)).Build();
        var position = PositionBuilder.New().WithId(1).Build();
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var request = new EmployeeRequest("John Doe", "987.826.470-03", new DateTime(2020, 1, 1), 1, 1);
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
        _employeePositionsRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmployeePosition>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _mapperMock.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }
}

