using AutoMapper;
using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Common.Pagination;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Providers;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Services.Base;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Services.Employees;

public class EmployeeService : BaseService, IEmployeeService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeePositionsRepository _employeePositionsRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<EmployeeFilter> _employeeFilterValidator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public EmployeeService(
        ICompanyRepository companyRepository,
        IEmployeeRepository employeeRepository,
        IEmployeePositionsRepository employeePositionsRepository,
        IPositionRepository positionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationContext notificationContext,
        IValidator<EmployeeFilter> employeeFilterValidator,
        IDateTimeProvider dateTimeProvider)
        : base(notificationContext)
    {
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
        _employeePositionsRepository = employeePositionsRepository;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _employeeFilterValidator = employeeFilterValidator;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<EmployeeResponse?> CreateAsync(EmployeeRequest request)
    {
        var cleanCpf = StringUtils.OnlyNumbers(request.Cpf);

        if (await _employeeRepository.GetByCpfAsync(cleanCpf) != null)
            return NotifyError<EmployeeResponse>("Cpf", "This employee is already registered.");

        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null) return NotifyError<EmployeeResponse>("Company", "Company not found.");

        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        if (position == null) return NotifyError<EmployeeResponse>("Position", "Position not found.");

        if (request.HireDate.HasValue && company.FoundationDate.HasValue &&
            request.HireDate.Value < company.FoundationDate.Value)
            return NotifyError<EmployeeResponse>("HireDate", "The hiring date cannot be earlier than the company's founding date.");

        var employee = new Employee(request.Name, request.Cpf, request.HireDate, request.CompanyId);
        if (!employee.Validation()) return AddDomainNotifications<EmployeeResponse>(employee.ValidationResult);

        await _employeeRepository.AddAsync(employee);

        var startDatePosition = request.HireDate ?? _dateTimeProvider.UtcNow;
        var employeeAndPosition = new EmployeePosition(employee, position, startDatePosition);
        if (!employeeAndPosition.Validation()) return AddDomainNotifications<EmployeeResponse>(employeeAndPosition.ValidationResult);

        await _employeePositionsRepository.AddAsync(employeeAndPosition);
        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyError<EmployeeResponse>("Commit", "Unable to save changes.");

        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<EmployeeResponse?> UpdateAsync(int id, EmployeeUpdateRequest request)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyError<EmployeeResponse>("Employee", "Employee not found.");

        var cpfClean = StringUtils.OnlyNumbers(request.Cpf);

        var existingWithCpf = await _employeeRepository.GetByCpfAsync(cpfClean);
        if (existingWithCpf != null && existingWithCpf.Id != id)
            return NotifyError<EmployeeResponse>("Cpf", "CPF already in use.");

        employee.Update(request.Name, cpfClean);

        if (!employee.Validation()) return AddDomainNotifications<EmployeeResponse>(employee.ValidationResult);

        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyError<EmployeeResponse>("Commit", "Unable to save changes.");

        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<bool> ChangePositionAsync(int employeeId, ChangeEmployeePositionRequest request)
    {
        var employee = await _employeeRepository.GetByIdWithCompanyAsync(employeeId);
        if (employee == null) return NotifyErrorBool("Employee", "Employee not found.");

        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        if (position == null) return NotifyErrorBool("Position", "Position not found.");

        var dateOfChange = _dateTimeProvider.UtcNow;

        if (employee.Company.FoundationDate.HasValue && dateOfChange < employee.Company.FoundationDate)
            return NotifyErrorBool("StartDate", $"The start date ({dateOfChange:dd/MM/yyyy}) cannot be earlier than the company foundation date ({employee.Company.FoundationDate:dd/MM/yyyy}).");

        if (await _employeePositionsRepository.HasEmployeeEverHeldPosition(employeeId, request.PositionId))
            return NotifyErrorBool("Position", "Employee has already held this position before.");

        var activePosition = await _employeePositionsRepository.GetActivePositionAsync(employeeId);
        if (activePosition != null)
        {
            activePosition.ClosePosition(dateOfChange);
            if (!activePosition.Validation())
                return AddDomainNotificationsBool(activePosition.ValidationResult);
        }

        var newPosition = new EmployeePosition(employee, position, dateOfChange);
        if (!newPosition.Validation())
            return AddDomainNotificationsBool(newPosition.ValidationResult);

        await _employeePositionsRepository.AddAsync(newPosition);
        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyErrorBool("Commit", "Unable to save changes.");

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyErrorBool("Employee", "Employee not found.");

        _employeeRepository.Delete(employee);
        
        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyErrorBool("Commit", "Unable to save changes.");
        
        return true;
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
        var validationResult = await _employeeFilterValidator.ValidateAsync(filter);

        if (!validationResult.IsValid)
        {
            AddValidationErrors(validationResult);
            return new PagedResponse<EmployeeResponse>(
                Enumerable.Empty<EmployeeResponse>(),
                0,
                filter.PageNumber,
                filter.PageSize);
        }

        var (employees, total) = await _employeeRepository.SearchAsync(filter);
        var mapperData = _mapper.Map<IEnumerable<EmployeeResponse>>(employees);

        return new PagedResponse<EmployeeResponse>(mapperData, total, filter.PageNumber, filter.PageSize);
    }
}