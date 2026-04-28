using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public class PositionServiceDeleteTests : PositionServiceTestBase
{
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

