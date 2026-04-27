using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Services.Companies;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<INotificationContext> _notificationContextMock;
    private readonly Mock<IValidator<CompanyFilter>> _companyFilterValidatorMock;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _notificationContextMock = new Mock<INotificationContext>();
        _companyFilterValidatorMock = new Mock<IValidator<CompanyFilter>>();
        
        _companyFilterValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CompanyFilter>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private CompanyService CreateService()
        => new(
            _mapperMock.Object,
            _companyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _notificationContextMock.Object,
            _companyFilterValidatorMock.Object
        );

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndAddNotification_WhenCnpjAlreadyExists()
    {
        var existing = CompanyBuilder.New()
            .WithCnpj("12.345.678/0001-99")
            .Build();
        
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync("12345678000199"))
            .ReturnsAsync(existing);

        var service = CreateService();
        var request = new CompanyRequest(
            "Test",
            "12.345.678/0001-99",
            new DateTime(2019, 01, 01)
        );
        
        var result = await service.CreateAsync(request);

        result.Should().BeNull();
        _notificationContextMock.Verify(
            n => n.AddNotification(
                nameof(Company.Cnpj),
                It.Is<string>(s => s.Contains("already registered."))
            ),
            Times.Once
        );
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Company>()), Times.Never);
      }

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndAddNotification_WhenDomainIsInvalid()
    {
        var request = new CompanyRequest(
            "Test",
            "123",
            new DateTime(2019, 01, 01)
        );

        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>()))
            .ReturnsAsync((Company?)null);

        var service = CreateService();
        var result = await service.CreateAsync(request);

        result.Should().BeNull();
        _companyRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Company>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(
            r => r.AddRange(
                It.Is<IEnumerable<ValidationFailure>>(failures =>
                    failures.Any(e =>
                        e.PropertyName == nameof(Company.Cnpj) &&
                        e.ErrorMessage.Contains("14 characters.")
                    )
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAndCommit_WhenValid()
    {
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        var mappedResponse = new CompanyResponse { Id = 1, Name = "Test", Cnpj = "11222333000181", };
        _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>())).Returns(mappedResponse);

        var service = CreateService();
        var request = new CompanyRequest("Test", "11.222.333/0001-81", new DateTime(2019, 1, 1));

        var result = await service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _companyRepositoryMock.Verify(r => r.AddAsync(It.Is<Company>(c => c.Name == "Test" && c.Cnpj == "11222333000181")), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalseAndAddNotification_WhenCompanyHasEmployees()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.HasEmployeesAsync(It.IsAny<int>())).ReturnsAsync(true);
        
        var service = CreateService();
        
        var result = await service.DeleteAsync(1);

        result.Should().BeFalse();
        _notificationContextMock.Verify(
            n => n.AddNotification(
                It.Is<string>(s => s == "Employees"),
                It.Is<string>(s => s.Contains("employees linked"))
            ), 
            Times.Once
        );
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
        _notificationContextMock.Verify(n => n.AddNotification(
            It.Is<string>(k => k == "Company"),
                It.Is<string>(m => m.Contains("not found"))
            ), 
            Times.Once
        );
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
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenDomainIsInvalid()
    {
        var company = CompanyBuilder.New().WithId(1).Build();
        
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);

        var request = new CompanyRequest(
            "Test",
            "123",
            null
        );
        
        var service = CreateService();
        var result = await service.UpdateAsync(1, request);

        result.Should().BeNull();
        _companyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Company>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _notificationContextMock.Verify(
            r => r.AddRange(
                It.Is<IEnumerable<ValidationFailure>>(failures =>
                    failures.Any(e =>
                        e.PropertyName == nameof(Company.Cnpj) &&
                        e.ErrorMessage.Contains("14 characters.")
                    )
                )
            ),
            Times.Once
        );
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenCompanyNotFound()
    {
        _companyRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Company?)null);
        var service = CreateService();
        var request = new CompanyRequest("Test", "12.345.678/0001-99", null);

        var result = await service.UpdateAsync(99, request);

        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(
            It.Is<string>(s => s == "Company"),
            It.Is<string>(s => s.Contains("not found"))
            ), 
            Times.Once
        );
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task 
        UpdateAsync_ShouldReturnNullAndAddNotification_WhenFoundationDateIsGreaterThanEmployeeHireDate()
    {
        var company = CompanyBuilder.New()
            .WithId(1)
            .WithFoundationDate(new DateTime(2019, 1, 1))
            .Build();
        
        var employee = EmployeeBuilder.New()
            .WithHireDate(new DateTime(2020, 1, 1))
            .Build();

        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetEarliestEmployeeHireDateAsync(1)).ReturnsAsync(employee.HireDate);
        
        var service = CreateService();
        var request = new CompanyRequest("Test", "12.345.678/0001-99", new DateTime(2020, 1, 2));
        
        var result = await service.UpdateAsync(1, request);
        
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(
            It.Is<string>(s => s == nameof(Company.FoundationDate)),
            It.Is<string>(s => s.Contains("employee"))
            ),
            Times.Once
        );
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Never);
    }
    
    [Fact]
    public async Task 
        UpdateAsync_ShouldUpdate_WhenFoundationDateIsLessThanEmployeeHireDate()
    {
        var company = CompanyBuilder.New()
            .WithId(1)
            .WithFoundationDate(new DateTime(2019, 1, 1))
            .Build();
        
        var employee = EmployeeBuilder.New()
            .WithHireDate(new DateTime(2020, 1, 1))
            .Build();

        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetEarliestEmployeeHireDateAsync(1)).ReturnsAsync(employee.HireDate);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>())).Returns(new CompanyResponse());
        
        var service = CreateService();
        var request = new CompanyRequest("Test", "76.550.315/0001-74", new DateTime(2019, 1, 1));
        
        var result = await service.UpdateAsync(1, request);
        
        result.Should().NotBeNull();
        _notificationContextMock.Verify(n => n.AddNotification(
                It.IsAny<string>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }
    
    [Fact]
    public async Task 
        UpdateAsync_ShouldUpdate_WhenFoundationDateIsEqualToEmployeeHireDate()
    {
        var company = CompanyBuilder.New()
            .WithId(1)
            .WithFoundationDate(new DateTime(2019, 1, 1))
            .Build();
        
        var employee = EmployeeBuilder.New()
            .WithHireDate(new DateTime(2020, 1, 1))
            .Build();

        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(r => r.GetEarliestEmployeeHireDateAsync(1)).ReturnsAsync(employee.HireDate);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<CompanyResponse>(It.IsAny<Company>())).Returns(new CompanyResponse());
        
        var service = CreateService();
        var request = new CompanyRequest("Test", "76.550.315/0001-74", new DateTime(2020, 1, 1));
        
        var result = await service.UpdateAsync(1, request);
        
        result.Should().NotBeNull();
        _notificationContextMock.Verify(n => n.AddNotification(
                It.IsAny<string>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNullAndAddNotification_WhenCnpjConflictExists()
    {
        var existingCompany = CompanyBuilder.New()
            .WithCnpj("11111111000111").WithId(1).Build();

        var companyToUpdate = CompanyBuilder.New()
            .WithCnpj("22222222000222").WithId(2).Build();

        var request = new CompanyRequest("New Name", "22.222.222/0002-22", null);

        _companyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCompany);
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync("22222222000222")).ReturnsAsync(companyToUpdate);
        var service = CreateService();

        var result = await service.UpdateAsync(1, request);

        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(
            It.Is<string>(s => s == "Cnpj"),
            It.Is<string>(s => s.Contains("already registered."))
            ), 
            Times.Once
        );
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
    public async Task GetByIdAsync_ShouldReturnResponse_WhenFound()
    {
        var company = CompanyBuilder.New()
            .WithName("Test")
            .Build();
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
        _notificationContextMock.Verify(n => n.AddNotification(
                It.Is<string>(s => s == "Company"), 
                It.Is<string>(s => s.Contains("not found"))
            ), 
            Times.Once
        );
        _mapperMock.Verify(m => m.Map<CompanyResponse>(It.IsAny<Company>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdCompanyAndEmployees_ShouldReturnNullAndAddNotification_WhenNotFound()
    {
        _companyRepositoryMock.Setup(r => r.GetByIdCompanyAndEmployees(1)).ReturnsAsync((Company?)null);
        
        var service = CreateService();

        var result = await service.GetByIdCompanyAndEmployees(1);
        
        result.Should().BeNull();
        _notificationContextMock.Verify(n => n.AddNotification(
                It.Is<string>(s => s == "Company"), 
                It.Is<string>(s => s.Contains("not found"))
            ), 
            Times.Once
        );
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

    [Fact]
    public async Task CreateAsync_ShouldReturnNullAndNotify_WhenCommitFails()
    {
        _companyRepositoryMock.Setup(r => r.GetByCnpjAsync(It.IsAny<string>())).ReturnsAsync((Company?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        var service = CreateService();
        var request = new CompanyRequest("Test", "11.222.333/0001-81", new DateTime(2019, 1, 1));
        var result = await service.CreateAsync(request);

        result.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _notificationContextMock.Verify(n => n.AddNotification("Commit", It.Is<string>(s => s.Contains("Unable to save"))), Times.Once);
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
