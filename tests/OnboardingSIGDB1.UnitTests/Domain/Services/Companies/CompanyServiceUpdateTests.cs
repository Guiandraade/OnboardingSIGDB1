using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public class CompanyServiceUpdateTests : CompanyServiceTestBase
{
    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenDomainIsInvalid()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);
        var request = new CompanyRequest("Test", "123", null);
        var service = CreateService();
        var result = await service.UpdateAsync(1, request);
        result.Should().BeNull();
        _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(r => r.AddRange(It.Is<IEnumerable<ValidationFailure>>(failures => failures.Any(e => e.PropertyName == nameof(Company.Cnpj) && e.ErrorMessage.Contains("14 characters.")))), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenCompanyNotFound()
    {
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Company?)null);
        var service = CreateService();
        var request = new CompanyRequest("Test", "12.345.678/0001-99", null);
        var result = await service.UpdateAsync(99, request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == "Company"), It.Is<string>(s => s.Contains("not found"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenFoundationDateIsGreaterThanEmployeeHireDate()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2019, 1, 1)).Build();
        var employee = EmployeeBuilder.New().WithHireDate(new DateTime(2020, 1, 1)).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetEarliestEmployeeHireDateAsync(1)).ReturnsAsync(employee.HireDate);
        var service = CreateService();
        var request = new CompanyRequest("Test", "12.345.678/0001-99", new DateTime(2020, 1, 2));
        var result = await service.UpdateAsync(1, request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == nameof(Company.FoundationDate)), It.Is<string>(s => s.Contains("employee"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenFoundationDateIsLessThanEmployeeHireDate()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2019, 1, 1)).Build();
        var employee = EmployeeBuilder.New().WithHireDate(new DateTime(2020, 1, 1)).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetEarliestEmployeeHireDateAsync(1)).ReturnsAsync(employee.HireDate);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>())).Returns(new CompanyResponse());
        var service = CreateService();
        var request = new CompanyRequest("Test", "76.550.315/0001-74", new DateTime(2019, 1, 1));
        var result = await service.UpdateAsync(1, request);
        result.Should().NotBeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenFoundationDateIsEqualToEmployeeHireDate()
    {
        var company = CompanyBuilder.New().WithId(1).WithFoundationDate(new DateTime(2019, 1, 1)).Build();
        var employee = EmployeeBuilder.New().WithHireDate(new DateTime(2020, 1, 1)).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetEarliestEmployeeHireDateAsync(1)).ReturnsAsync(employee.HireDate);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>())).Returns(new CompanyResponse());
        var service = CreateService();
        var request = new CompanyRequest("Test", "76.550.315/0001-74", new DateTime(2020, 1, 1));
        var result = await service.UpdateAsync(1, request);
        result.Should().NotBeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenCnpjConflictExists()
    {
        var existingCompany = CompanyBuilder.New().WithCnpj("11111111000111").WithId(1).Build();
        var companyToUpdate = CompanyBuilder.New().WithCnpj("22222222000222").WithId(2).Build();
        var request = new CompanyRequest("New Name", "22.222.222/0002-22", null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCompany);
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync("22222222000222")).ReturnsAsync(companyToUpdate);
        var service = CreateService();
        var result = await service.UpdateAsync(1, request);
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(It.Is<string>(s => s == "Cnpj"), It.Is<string>(s => s.Contains("already registered."))), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndCommit_WhenValid()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        var request = new CompanyRequest("New Name", "11.444.777/0001-61", null);
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>())).Returns(new CompanyResponse());
        var service = CreateService();
        var result = await service.UpdateAsync(1, request);
        result.Should().NotBeNull();
        company.Name.Should().Be("New Name");
        company.Cnpj.Should().Be("11444777000161");
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);
        var service = CreateService();
        var request = new CompanyRequest("New Name", "11.444.777/0001-61", null);
        var result = await service.UpdateAsync(1, request);
        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
    }
}
