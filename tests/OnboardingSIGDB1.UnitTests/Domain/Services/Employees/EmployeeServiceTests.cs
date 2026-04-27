using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.EmployeeAndPositions.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Providers;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Services.Employees;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public class EmployeeServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IEmployeePositionsRepository> _employeePositionsRepositoryMock;
    private readonly Mock<IPositionRepository> _positionRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<INotificationContext> _notificationContextMock;
    private readonly Mock<IValidator<EmployeeFilter>> _employeeFilterValidatorMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public EmployeeServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _employeePositionsRepositoryMock = new Mock<IEmployeePositionsRepository>();
        _positionRepositoryMock = new Mock<IPositionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _notificationContextMock = new Mock<INotificationContext>();
        _employeeFilterValidatorMock = new Mock<IValidator<EmployeeFilter>>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

        _employeeFilterValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<EmployeeFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private EmployeeService CreateService()
        => new(
            _companyRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _employeePositionsRepositoryMock.Object,
            _positionRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _notificationContextMock.Object,
            _employeeFilterValidatorMock.Object,
            _dateTimeProviderMock.Object
        );

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
        _notificationContextMock.Verify((n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>())), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _employeeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCompanyNotFound()
    {
        _employeeRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Company?)null);

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
        _mapperMock.Setup(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()))
            .Returns(new EmployeeResponse { Name = "John Doe" });

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
        _mapperMock.Setup(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()))
            .Returns(new EmployeeResponse { Name = "Updated" });

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
        _mapperMock.Setup(m => m.Map<EmployeeAndPositionsResponse>(employee))
            .Returns(new EmployeeAndPositionsResponse { Id = 1 });

        var service = CreateService();
        var result = await service.GetHistoryAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mapperMock.Verify(m => m.Map<EmployeeAndPositionsResponse>(It.IsAny<Employee>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
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
        var company =  CompanyBuilder.New().Build();
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
    
    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyAndNotify_WhenFilterIsInvalid()
    {
        var failures = new List<ValidationFailure>
        {
            new("PageNumber", "Page number must be greater than 0.")
        };
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
