using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Positions.Commands;
using OnboardingSIGDB1.Domain.Dto.Positions.Responses;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public class PositionServiceUpdateTests : PositionServiceTestBase
{
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
        _mapperMock.Setup(m => m.Map<PositionResponse>(It.IsAny<Position>())).Returns(new PositionResponse { Id = position.Id, Description = position.Description });
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
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == nameof(Position.Description)), It.Is<string>(s => s.Contains("uses this description."))), Times.Once);
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
}

