using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Responses;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Positions;

public class PositionServiceSearchTests : PositionServiceTestBase
{
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
        var invalidFilter = new PositionFilter { PageNumber = 0, PageSize = 0 };
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
}

