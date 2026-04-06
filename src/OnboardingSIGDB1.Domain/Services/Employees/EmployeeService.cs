using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.filters.Validators;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmployeeRepository _employeeRepository; 
    private readonly IPositionRepository _positionRepository; 
    private readonly IUnitOfWork _unitOfWork;                
    private readonly IMapper _mapper;
    private readonly INotificationContext _notificationContext;
    
    public EmployeeService(
        ICompanyRepository companyRepository,
        IEmployeeRepository employeeRepository,
        IPositionRepository positionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationContext notificationContext
    )
    {
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationContext = notificationContext;
    }

    private T? NotifyError<T>(string key, string message) where T : class
    {
        _notificationContext.AddNotification(key, message);
        return null;
    }
    
    private T? AddDomainNotifications<T>(Employee employee) where T : class
    {
        _notificationContext.AddRange(employee.Notifications);
        return null;
    }
    
    private bool NotifyErrorBool(string key, string message)
    {
        _notificationContext.AddNotification(key, message);
        return false;
    }
    
    public async Task<EmployeeResponse?> CreateAsync(EmployeeRequest request)
    {
        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);

        if (company == null) return NotifyError<EmployeeResponse>("Company", "Company not found.");
        if (position == null) return NotifyError<EmployeeResponse>("Position", "Position not found.");
        
        var cleanCpf = StringUtils.OnlyNumbers(request.Cpf);
        if(await _employeeRepository.GetByCpfAsync(cleanCpf) != null)
            return NotifyError<EmployeeResponse>("Employee", "This employee is already registered.");
        
        var employee = new Employee(request.Name, request.Cpf, request.HireDate, request.CompanyId, position);
        if (!employee.Validation()) return AddDomainNotifications<EmployeeResponse>(employee);
        
        await _employeeRepository.AddAsync(employee);
        await _unitOfWork.CommitAsync();
        
        var result = await _employeeRepository.GetByIdAsync(employee.Id);
        
        return _mapper.Map<EmployeeResponse>(result);
    }

    public async Task<EmployeeResponse?> UpdateAsync(int id, EmployeeUpdateRequest request)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyError<EmployeeResponse>("Employee", "Employee not found.");
        
        var cpfClean = StringUtils.OnlyNumbers(request.Cpf);
        var existingWithCpf = await _employeeRepository.GetByCpfAsync(cpfClean);
        if (existingWithCpf != null && existingWithCpf.Id != id)
            return NotifyError<EmployeeResponse>("Cpf", "This CPF is already in use by another employee.");

        var newPosition = await _positionRepository.GetByIdAsync(request.PositionId);
        if (newPosition == null) return NotifyError<EmployeeResponse>("Position", "Position not found.");
        
        employee.Update(request.Name, cpfClean, request.HireDate, newPosition);
        
        if (!employee.IsValid) return AddDomainNotifications<EmployeeResponse>(employee);
        
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyErrorBool("Employee", "Employee not found.");
        
        _employeeRepository.Delete(employee);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyError<EmployeeResponse>("Employee", "Employee not found.");
        
        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeAndPositionsResponse?> GetHistoryAsync(int id)
    {
        var employee = await _employeeRepository.GetHistoryAsync(id);
        if (employee == null) return NotifyError<EmployeeAndPositionsResponse>("Employee", "Employee not found.");
        
        return _mapper.Map<EmployeeAndPositionsResponse>(employee);
    }
    
    public async Task<PagedResponse<EmployeeResponse>> SearchAsync(EmployeeFilter filter)
    {
        var validator = new EmployeeFilterValidator();
        var validationResult = await validator.ValidateAsync(filter);

        if (!validationResult.IsValid)
        {
            _notificationContext.AddRange(validationResult.Errors);

            return new PagedResponse<EmployeeResponse>(
                Enumerable.Empty<EmployeeResponse>(),
                0,
                filter.PageNumber,
                filter.PageSize
            );
        }

        var (employee, total) = await _employeeRepository.SearchAsync(filter);

        var mapperData = _mapper.Map<IEnumerable<EmployeeResponse>>(employee);

        return new PagedResponse<EmployeeResponse>(
            mapperData,
            total,
            filter.PageNumber,
            filter.PageSize
        );
    }
}