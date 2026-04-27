using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Services.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public class PositionServiceTests
{
    private readonly Mock<IPositionRepository> _positionRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<INotificationContext> _notificationContextMock;
    private readonly Mock<IValidator<PositionFilter>> _positionFilterValidatorMock;

    public PositionServiceTests()
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

    private PositionService CreateService() 
        => new(
            _mapperMock.Object,
            _positionRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationContextMock.Object,
            _positionFilterValidatorMock.Object
        );

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionAlreadyExists()
    {
        var position = PositionBuilder.New()
            .WithDescription("Developer")
            .Build();
        
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync("Developer"))
            .ReturnsAsync(position);
        
        var service = CreateService();
        var request = new PositionRequest( Description: "Developer");
        var result = await service.CreateAsync(request);

        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(
            n => n.AddNotification(
                nameof(Position.Description),
                It.Is<string>(s => s.Contains("exists."))
            ), 
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionIsInvalid()
    {
        var service = CreateService();
        var request = new PositionRequest(Description: "");
        var result = await service.CreateAsync(request);
        
        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByDescriptionAsync(""), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(
            r => r.AddRange(
                It.Is<IEnumerable<ValidationFailure>>(failures => 
                    failures.Any(e => 
                        e.PropertyName == nameof(Position.Description) && 
                        e.ErrorMessage.Contains("required")
                    )
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionIsNull()
    {
        var service = CreateService();
        var request = new PositionRequest(Description: null!);
        var result = await service.CreateAsync(request);
        
        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByDescriptionAsync(""), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(
            r => r.AddRange(
                It.Is<IEnumerable<ValidationFailure>>(failures => 
                    failures.Any(e => 
                        e.PropertyName == nameof(Position.Description) && 
                        e.ErrorMessage.Contains("required")
                    )
                )
            ),
            Times.Once
        );
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreatePositionAndReturnResponse_WhenDescriptionIsUnique()
    {
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>()))
            .ReturnsAsync((Position?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<PositionResponse>(It.IsAny<Position>()))
            .Returns((Position position) => new PositionResponse { Description = position.Description });
        
        var service = CreateService();
        var request = new PositionRequest(Description: "Developer");
        var result = await service.CreateAsync(request);
        
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new PositionResponse { Description = "Developer" });
        _positionRepositoryMock.Verify(r => r.AddAsync(It.Is<Position>(p => p.Description == "Developer")), Times.Once);
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.Is<Position>(p => p.Description == "Developer")), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenPositionHasEmployeesLinked()
    {
        var position = PositionBuilder.New().WithId(1).Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _positionRepositoryMock.Setup(r => r.HasEmployeesAsync(1)).ReturnsAsync(true);

        var service = CreateService();
        var result = await service.DeleteAsync(1);
        
        result.Should().BeFalse();
        _positionRepositoryMock.Verify(r => r.Delete(position), Times.Never);
        _notificationContextMock.Verify(n => n.AddNotification(nameof(Position), It.Is<string>(s => s.Contains("linked to employees"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenPositionNotFound()
    {
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Position?)null);

        var service = CreateService();
        var result = await service.DeleteAsync(1);
        
        result.Should().BeFalse();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _positionRepositoryMock.Verify(r => r.Delete(It.IsAny<Position>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddNotification(nameof(Position), It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenPositionIsValid()
    {
        var position = PositionBuilder.New().WithId(1).Build();

        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _positionRepositoryMock.Setup(r => r.HasEmployeesAsync(1)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        
        result.Should().BeTrue();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _positionRepositoryMock.Verify(r => r.HasEmployeesAsync(1), Times.Once);
        _positionRepositoryMock.Verify(r => r.Delete(It.IsAny<Position>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPositionNotFound()
    {
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Position?)null);
        
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        
        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.IsAny<Position>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddNotification(nameof(Position), It.Is<string>(s => s.Contains("not found"))), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnResponse_WhenPositionIsValid()
    {
        var expectedPosition = PositionBuilder.New().WithId(1).Build();
        
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedPosition);
        _mapperMock.Setup(m => m.Map<PositionResponse>(It.IsAny<Position>())).Returns(new PositionResponse{ Id = expectedPosition.Id, Description = expectedPosition.Description });
        
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        
        result.Should().NotBeNull();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.IsAny<Position>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenPositionNotFound()
    {
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Position?)null);
        
        var service = CreateService();
        var result = await service.UpdateAsync(1, new PositionRequest(Description: "Developer"));

        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(nameof(Position), It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.IsAny<Position>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldCreatePositionAndReturnResponse_WhenDescriptionIsUnique()
    {
        var position = PositionBuilder.New().WithId(1).Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(position);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<PositionResponse>(It.IsAny<Position>()))
            .Returns(new PositionResponse { Id = position.Id, Description = position.Description });
        
        var service = CreateService();
        var result = await service.UpdateAsync(1, new PositionRequest(Description: "Developer"));
        
        result.Should().NotBeNull();
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.IsAny<Position>()), Times.Once);
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _positionRepositoryMock.Verify(r => r.GetByDescriptionAsync(It.IsAny<string>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionAlreadyExists()
    {
        var positionToUpdate = PositionBuilder.New().WithDescription("Analyst").Build();
        var existingPositionWithSameDescription = PositionBuilder.New().WithDescription("Developer").Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(positionToUpdate);
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync("Developer")).ReturnsAsync(existingPositionWithSameDescription);
        
        var service = CreateService();
        var result = await service.UpdateAsync(1, new PositionRequest(Description: "Developer"));

        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _positionRepositoryMock.Verify(r => r.GetByDescriptionAsync(It.IsAny<string>()), Times.Once);
        _notificationContextMock.Verify(
            n => n.AddNotification(
                It.Is<string>(s => s == nameof(Position.Description)), 
                It.Is<string>(s => s.Contains("uses this description.")
                )
            ),
            Times.Once
        );
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.IsAny<Position>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionIsInvalid()
    {
        var position = PositionBuilder.New().Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(position);
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync((Position?)null);
        
        var service = CreateService();
        var result = await service.UpdateAsync(1, new PositionRequest(Description: ""));
        
        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _positionRepositoryMock.Verify(r => r.GetByDescriptionAsync(It.IsAny<string>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionIsNull()
    {
        var position = PositionBuilder.New().Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(position);
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync((Position?)null);
        
        var service = CreateService();
        var result = await service.UpdateAsync(1, new PositionRequest(Description: null!));
        
        result.Should().BeNull();
        _positionRepositoryMock.Verify(r => r.GetByDescriptionAsync(""), Times.Once);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyAndAddNotification_WhenFilterIsInvalid()
    {
        var failures = new List<ValidationFailure>
        {
            new("PageNumber", "Page number must be greater than 0."),
            new("PageSize", "Page size must be between 1 and 100.")
        };
        
        _positionFilterValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<PositionFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        
        var service = CreateService();
        var invalidFilter = new PositionFilter { PageNumber =  0, PageSize = 0 };
        
        var result = await service.SearchAsync(invalidFilter);

        result.Data.Should().BeEmpty();
        result.Total.Should().Be(0);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPagedResponse_WhenFilterIsValid()
    {
        var filter = new PositionFilter() { PageNumber = 1, PageSize = 10 };
        var companies = new List<Position> { PositionBuilder.New().WithDescription("Test").Build() };
        var response = new List<PositionResponse> { new() { Description = "Test" } };
        
        _positionRepositoryMock.Setup(r => r.SearchAsync(filter)).ReturnsAsync((companies, 1));
        _mapperMock.Setup(m => m.Map<IEnumerable<PositionResponse>>(companies)).Returns(response);
        
        var service = CreateService();
        var result = await service.SearchAsync(filter);
        
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Total.Should().Be(1);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        _positionFilterValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<PositionFilter>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>()))
            .ReturnsAsync((Position?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.CreateAsync(new PositionRequest(Description: "Developer"));

        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        var position = PositionBuilder.New().WithId(1).Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync((Position?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.UpdateAsync(1, new PositionRequest(Description: "Developer"));

        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndNotify_WhenCommitFails()
    {
        var position = PositionBuilder.New().WithId(1).Build();
        _positionRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
        _positionRepositoryMock.Setup(r => r.HasEmployeesAsync(1)).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.DeleteAsync(1);

        result.Should().BeFalse();
        _positionRepositoryMock.Verify(r => r.Delete(position), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
    }
}
