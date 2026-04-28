using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Services.Companies;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public abstract class CompanyServiceTestBase
{
    protected readonly Mock<ICompanyRepository> _companyRepositoryMock;
    protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
    protected readonly Mock<IMapper> _mapperMock;
    protected readonly Mock<INotificationContext> _notificationContextMock;
    protected readonly Mock<IValidator<CompanyFilter>> _companyFilterValidatorMock;

    protected CompanyServiceTestBase()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _notificationContextMock = new Mock<INotificationContext>();
        _companyFilterValidatorMock = new Mock<IValidator<CompanyFilter>>();

        _companyFilterValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CompanyFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    protected CompanyService CreateService()
        => new(
            _mapperMock.Object,
            _companyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationContextMock.Object,
            _companyFilterValidatorMock.Object
        );
}

