using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public class PositionServiceCreateTests : PositionServiceTestBase
{
    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndAddNotification_WhenDescriptionAlreadyExists()
    {
        var position = PositionBuilder.New().WithDescription("Developer").Build();
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync("Developer")).ReturnsAsync(position);
        var service = CreateService();
        var request = new PositionRequest(Description: "Developer");
        var result = await service.CreateAsync(request);
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(n => n.AddNotification(nameof(Position.Description), It.Is<string>(s => s.Contains("exists."))), Times.Once);
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
        _notificationContextMock.Verify(r => r.AddRange(It.Is<IEnumerable<ValidationFailure>>(failures => failures.Any(e => e.PropertyName == nameof(Position.Description) && e.ErrorMessage.Contains("required")))), Times.Once);
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
        _notificationContextMock.Verify(r => r.AddRange(It.Is<IEnumerable<ValidationFailure>>(failures => failures.Any(e => e.PropertyName == nameof(Position.Description) && e.ErrorMessage.Contains("required")))), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreatePositionAndReturnResponse_WhenDescriptionIsUnique()
    {
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync((Position?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<PositionResponse>(It.IsAny<Position>())).Returns((Position position) => new PositionResponse { Description = position.Description });
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
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        _positionRepositoryMock.Setup(r => r.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync((Position?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var result = await service.CreateAsync(new PositionRequest(Description: "Developer"));
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
    }
}

