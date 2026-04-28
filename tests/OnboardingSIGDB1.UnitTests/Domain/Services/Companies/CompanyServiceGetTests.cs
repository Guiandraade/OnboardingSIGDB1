using FluentAssertions;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public class CompanyServiceGetTests : CompanyServiceTestBase
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnResponse_WhenFound()
    {
        var company = CompanyBuilder.New().WithName("Test").Build();
        var response = new CompanyResponse { Id = 1, Name = "Test" };
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(company);
        _mapperMock.Setup(m => m.Map<CompanyResponse>(company)).Returns(response);
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        result.Should().NotBeNull();
        result.Should().Be(response);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullAndAddNotification_WhenNotFound()
    {
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Company?)null);
        var service = CreateService();
        var result = await service.GetByIdAsync(1);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == "Company"), It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdCompanyAndEmployees_ShouldReturnNullAndAddNotification_WhenNotFound()
    {
        _companyRepositoryMock.Setup(r => r.GetByIdCompanyAndEmployees(1)).ReturnsAsync((Company?)null);
        var service = CreateService();
        var result = await service.GetByIdCompanyAndEmployees(1);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == "Company"), It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyAndEmployeesResponse>(It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdCompanyAndEmployees_ShouldReturnResponse_WhenFound()
    {
        var company = CompanyBuilder.New().Build();
        _companyRepositoryMock.Setup(r => r.GetByIdCompanyAndEmployees(1)).ReturnsAsync(company);
        var response = new CompanyAndEmployeesResponse();
        _mapperMock.Setup(m => m.Map<CompanyAndEmployeesResponse>(company)).Returns(response);
        var service = CreateService();
        var result = await service.GetByIdCompanyAndEmployees(1);
        result.Should().NotBeNull();
        _mapperMock.Verify(m => m.Map<CompanyAndEmployeesResponse>(company), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

