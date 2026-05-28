using FluentAssertions;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Companies.Responses;
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
    public async Task GetCompanyWithEmployeesByIdAsync_ShouldReturnNullAndAddNotification_WhenNotFound()
    {
        _companyRepositoryMock.Setup(r => r.GetCompanyWithEmployeesByIdAsync(1)).ReturnsAsync((Company?)null);
        var service = CreateService();

        var result = await service.GetCompanyWithEmployeesByIdAsync(1);

        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == "Company"), It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyAndEmployeesResponse>(It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task GetCompanyWithEmployeesByIdAsync_ShouldReturnResponse_WhenFound()
    {
        var company = CompanyBuilder.New().Build();
        var response = new CompanyAndEmployeesResponse();
        _companyRepositoryMock.Setup(r => r.GetCompanyWithEmployeesByIdAsync(1)).ReturnsAsync(company);
        _mapperMock.Setup(m => m.Map<CompanyAndEmployeesResponse>(company)).Returns(response);
        var service = CreateService();

        var result = await service.GetCompanyWithEmployeesByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().Be(response);
        _mapperMock.Verify(m => m.Map<CompanyAndEmployeesResponse>(company), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

