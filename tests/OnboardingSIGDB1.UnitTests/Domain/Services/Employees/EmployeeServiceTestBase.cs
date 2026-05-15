using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Providers;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Services.Employees;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Employees;

public abstract class EmployeeServiceTestBase
{
    protected readonly Mock<ICompanyRepository> _companyRepositoryMock;
    protected readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    protected readonly Mock<IEmployeePositionsRepository> _employeePositionsRepositoryMock;
    protected readonly Mock<IPositionRepository> _positionRepositoryMock;
    protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
    protected readonly Mock<IMapper> _mapperMock;
    protected readonly Mock<INotificationContext> _notificationContextMock;
    protected readonly Mock<IValidator<EmployeeFilter>> _employeeFilterValidatorMock;
    protected readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    protected EmployeeServiceTestBase()
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

    protected EmployeeService CreateService()
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
}

