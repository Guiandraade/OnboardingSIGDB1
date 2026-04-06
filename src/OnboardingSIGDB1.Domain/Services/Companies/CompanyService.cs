using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.filters.Validators;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Services.Companies;

public class CompanyService : ICompanyService
{
    private readonly INotificationContext _notificationContext;
    private readonly IMapper _mapper;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;    
    
    public CompanyService(
        IMapper mapper, 
        ICompanyRepository repository, 
        IUnitOfWork unitOfWork,  
        INotificationContext notificationContext)
    {
        _mapper = mapper;
        _companyRepository = repository;
        _unitOfWork = unitOfWork;
        _notificationContext = notificationContext;
    }

    private T? NotifyError<T>(string key, string message) where T : class
    {
        _notificationContext.AddNotification(key, message);
        return null;
    }
    
    private T? AddDomainNotifications<T>(Company company) where T : class
    {
        _notificationContext.AddRange(company.Notifications);
        return null;
    }

    private bool NotifyErrorBool(string key, string message)
    {
        _notificationContext.AddNotification(key, message);
        return false;
    }
    
    public async Task<CompanyResponse?> CreateAsync(CompanyRequest request)
    {
        var cnpjClean = StringUtils.OnlyNumbers(request.Cnpj);
        var existingCnpj = await _companyRepository.GetByCnpjAsync(cnpjClean);
        if (existingCnpj != null) return NotifyError<CompanyResponse>("Cnpj", "A company with this CNPJ is already registered.");

        var company = new Company(request.Name, cnpjClean, request.FoundationDate);
        
        if (!company.Validation()) return AddDomainNotifications<CompanyResponse>(company);
        
        await _companyRepository.AddAsync(company);
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<CompanyResponse>(company);
    }

    public async Task<CompanyResponse?> UpdateAsync(int id, CompanyRequest request)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null) return NotifyError<CompanyResponse>("Company", "Company not found.");
        
        var cnpjClean = StringUtils.OnlyNumbers(request.Cnpj);
        
        var existingWithCnpj = await _companyRepository.GetByCnpjAsync(request.Cnpj);
        if (existingWithCnpj != null && existingWithCnpj.Id != company.Id) 
            return NotifyError<CompanyResponse>("Cnpj", "A company with this CNPJ is already registered.");
        
        company.Update(request.Name, cnpjClean, request.FoundationDate);
        
        if (!company.IsValid) return AddDomainNotifications<CompanyResponse>(company);
        
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<CompanyResponse>(company);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null) return NotifyErrorBool("Company", "Company not found.");
        
        var existingEmployees = await _companyRepository.HasEmployeesAsync(company.Id);
        if (existingEmployees) 
            return NotifyErrorBool("Employees", "The company cannot be excluded because there are employees linked to it.");

        _companyRepository.Delete(company);
        
        return await _unitOfWork.CommitAsync();;
    }

    public async Task<CompanyResponse?> GetByIdAsync(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null) return NotifyError<CompanyResponse>("Company", "Company not found.");
        
        return _mapper.Map<CompanyResponse>(company);
    }

    public async Task<CompanyAndEmployeesResponse?> GetByIdCompanyAndEmployees(int id)
    {
        var company = await _companyRepository.GetByIdCompanyAndEmployees(id);
        if (company == null) return NotifyError<CompanyAndEmployeesResponse>("Company", "Company not found.");
        
        return _mapper.Map<CompanyAndEmployeesResponse>(company);
    }

    public async Task<PagedResponse<CompanyResponse>> SearchAsync(CompanyFilter filter)
    {
        var validator = new CompanyFilterValidator();
        var validationResult = await validator.ValidateAsync(filter);

        if (!validationResult.IsValid)
        {
            _notificationContext.AddRange(validationResult.Errors);

            return new PagedResponse<CompanyResponse>(
                Enumerable.Empty<CompanyResponse>(),
                0,
                filter.PageNumber,
                filter.PageSize
            );
        }
        
        var (company, total) = await _companyRepository.SearchAsync(filter);
        
        var mapperData = _mapper.Map<IEnumerable<CompanyResponse>>(company);
        
        return new PagedResponse<CompanyResponse>(
            mapperData,
            total,
            filter.PageNumber,
            filter.PageSize
        );
    }
}