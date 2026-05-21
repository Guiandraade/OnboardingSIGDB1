using FluentAssertions;
using Moq;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public class CompanyServiceDeleteTests : CompanyServiceTestBase
{
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndAddNotification_WhenCompanyHasEmployees()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.HasEmployeesAsync(It.IsAny<int>())).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == "Employees"), It.Is<string>(s => s.Contains("employees linked"))), Times.Once);
        _companyRepositoryMock.Verify(r => r.Delete(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndAddNotification_WhenCompanyDoesNotExist()
    {
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Company?)null);
        var service = CreateService();
        var result = await service.DeleteAsync(2);
        result.Should().BeFalse();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(k => k == "Company"), It.Is<string>(m => m.Contains("not found"))), Times.Once);
        _companyRepositoryMock.Verify(c => c.Delete(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldDeleteAndCommit_WhenValid()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.HasEmployeesAsync(It.IsAny<int>())).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        result.Should().BeTrue();
        _companyRepositoryMock.Verify(r => r.Delete(company), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndNotify_WhenCommitFails()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.HasEmployeesAsync(It.IsAny<int>())).ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var result = await service.DeleteAsync(1);
        result.Should().BeFalse();
        _companyRepositoryMock.Verify(r => r.Delete(company), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
    }
}

