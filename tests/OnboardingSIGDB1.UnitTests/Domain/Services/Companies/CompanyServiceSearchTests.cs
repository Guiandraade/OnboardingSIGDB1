using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public class CompanyServiceSearchTests : CompanyServiceTestBase
{
    [Fact]
    public async Task SearchAsync_ShouldReturnEmptyAndAddNotification_WhenFilterIsInvalid()
    {
        var failures = new List<ValidationFailure>
        {
            new("PageNumber", "Page number must be greater than 0."),
            new("PageSize", "Page size must be between 1 and 100.")
        };
        _companyFilterValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CompanyFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));
        var service = CreateService();
        var invalidFilter = new CompanyFilter { PageNumber = 0, PageSize = 0 };
        var result = await service.SearchAsync(invalidFilter);
        result.Data.Should().BeEmpty();
        result.Total.Should().Be(0);
        _notificationContextMock.Verify(n => n.AddRange(It.IsAny<IEnumerable<ValidationFailure>>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPagedResponse_WhenFilterIsValid()
    {
        var filter = new CompanyFilter { PageNumber = 1, PageSize = 10 };
        var companies = new List<Company> { CompanyBuilder.New().WithName("Test").WithCnpj("12345678000199").Build() };
        var responses = new List<CompanyResponse> { new() { Name = "Test" } };
        _companyRepositoryMock.Setup(r => r.SearchAsync(filter)).ReturnsAsync((companies, 1));
        _mapperMock.Setup(m => m.Map<IEnumerable<CompanyResponse>>(companies)).Returns(responses);
        var service = CreateService();
        var result = await service.SearchAsync(filter);
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Total.Should().Be(1);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        _companyFilterValidatorMock.Verify(v => v.ValidateAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
    }
}

