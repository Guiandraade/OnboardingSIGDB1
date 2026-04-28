using FluentAssertions;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public class PositionServiceGetTests : PositionServiceTestBase
{
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
        _mapperMock.Setup(m => m.Map<PositionResponse>(It.IsAny<Position>())).Returns(new PositionResponse { Id = expectedPosition.Id, Description = expectedPosition.Description });
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        result.Should().NotBeNull();
        _positionRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _mapperMock.Verify(m => m.Map<PositionResponse>(It.IsAny<Position>()), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

