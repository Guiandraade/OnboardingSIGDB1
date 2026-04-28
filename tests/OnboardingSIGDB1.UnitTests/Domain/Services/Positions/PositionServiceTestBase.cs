using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Services.Positions;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public abstract class PositionServiceTestBase
{
    protected readonly Mock<IPositionRepository> _positionRepositoryMock;
    protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
    protected readonly Mock<IMapper> _mapperMock;
    protected readonly Mock<INotificationContext> _notificationContextMock;
    protected readonly Mock<IValidator<PositionFilter>> _positionFilterValidatorMock;

    protected PositionServiceTestBase()
    {
        _positionRepositoryMock = new Mock<IPositionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _notificationContextMock = new Mock<INotificationContext>();
        _positionFilterValidatorMock = new Mock<IValidator<PositionFilter>>();

        _positionFilterValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<PositionFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    protected PositionService CreateService()
        => new(
            _mapperMock.Object,
            _positionRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationContextMock.Object,
            _positionFilterValidatorMock.Object
        );
}

