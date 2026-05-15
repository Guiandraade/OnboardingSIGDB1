using FluentAssertions;
using FluentValidation.Results;
using Moq;
using OnboardingSIGDB1.Domain.Dto.Companies.Commands;
using OnboardingSIGDB1.Domain.Dto.Companies.Responses;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.UnitTests.Builders;

namespace OnboardingSIGDB1.UnitTests.Domain.Services.Companies;

public class CompanyServiceCreateTests : CompanyServiceTestBase
{
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
}

