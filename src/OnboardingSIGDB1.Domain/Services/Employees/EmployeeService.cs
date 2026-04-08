using AutoMapper;
using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.EmployeeAndPositions.Request;
using OnboardingSIGDB1.Domain.Dto.EmployeeAndPositions.Response;
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
    private readonly IEmployeePositionsRepository _employeePositionsRepository;
    private readonly IPositionRepository _positionRepository; 
    private readonly IUnitOfWork _unitOfWork;                
    private readonly IMapper _mapper;
    private readonly INotificationContext _notificationContext;
    
    public EmployeeService(
        ICompanyRepository companyRepository,
        IEmployeeRepository employeeRepository,
        IEmployeePositionsRepository employeePositionsRepository,
        IPositionRepository positionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationContext notificationContext
    )
    {
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
        _employeePositionsRepository = employeePositionsRepository;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationContext = notificationContext;
    }
    
    private Result<T> NotifyFailure<T>(IEnumerable<(string Key, string Message)> errors)
    {
        foreach (var error in errors)
            _notificationContext.AddNotification(error.Key, error.Message);
        
        return Result<T>.Failure("Validation failed");
    }
    
    private Result<T> NotifyFailure<T>(string message, string key = "Error")
    {
        _notificationContext.AddNotification(key, message);
        return Result<T>.Failure(message);
    }
    
    private Result NotifyFailure(string message, string key = "Error")
    {
        _notificationContext.AddNotification(key, message);
        return Result.Failure(message);
    }
    
    public async Task<Result<EmployeeResponse>> CreateAsync(EmployeeRequest request)
    {
        var cleanCpf = StringUtils.OnlyNumbers(request.Cpf);
        
        if (await _employeeRepository.GetByCpfAsync(cleanCpf) != null)
            return NotifyFailure<EmployeeResponse>("This employee is already registered.", "Cpf");

        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        
        if (company == null) return NotifyFailure<EmployeeResponse>("Company not found.", "Company");
        if (position == null) return NotifyFailure<EmployeeResponse>("Position not found.", "Position");
        
        if (request.HireDate.HasValue && request.HireDate < company.FoundationDate)
        {
            return NotifyFailure<EmployeeResponse>("The hiring date cannot be earlier than the company's founding date.", "HireDate");
        }
        
        var employee = new Employee(request.Name, request.Cpf, request.HireDate, request.CompanyId);
        if (!employee.Validation())
        {
            return NotifyFailure<EmployeeResponse>(
                errors: employee.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage))
            );
        }
        
        await _employeeRepository.AddAsync(employee);
        
        var employeeAndPosition = new EmployeePosition(employee, position, DateTime.UtcNow);
        if (!employeeAndPosition.Validation())
        {
            return NotifyFailure<EmployeeResponse>(
                errors: employeeAndPosition.ValidationResult.Errors
                    .Select(e => (e.PropertyName, e.ErrorMessage))
            );
        }
        
        await _employeePositionsRepository.AddAsync(employeeAndPosition);
        await _unitOfWork.CommitAsync();
        
        var result = await _employeeRepository.GetByIdAsync(employee.Id);

        var response = _mapper.Map<EmployeeResponse>(result);
        return Result<EmployeeResponse>.Success(response);
    }
    
    public async Task<Result<EmployeeResponse>> UpdateAsync(int id, EmployeeUpdateRequest request)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyFailure<EmployeeResponse>("Employee not found.", "Employee");
        
        var cpfClean = StringUtils.OnlyNumbers(request.Cpf);
        
        var existingWithCpf = await _employeeRepository.GetByCpfAsync(cpfClean);
        if (existingWithCpf != null && existingWithCpf.Id != id)
            return NotifyFailure<EmployeeResponse>("CPF already in use.", "Cpf");
        
        employee.Update(request.Name, cpfClean);

        if (!employee.Validation())
        {
            return NotifyFailure<EmployeeResponse>(
                errors: employee.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage))
            );
        }
        
        await _unitOfWork.CommitAsync();

        var result = await _employeeRepository.GetByIdAsync(employee.Id);
        
        var response = _mapper.Map<EmployeeResponse>(result);
        return Result<EmployeeResponse>.Success(response);
    }
    
    public async Task<Result> ChangePositionAsync(int employeeId, ChangeEmployeePositionRequest request)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null) return NotifyFailure("Employee not found.", "Employee");

        var position = await _positionRepository.GetByIdAsync(request.PositionId);
        if (position == null) return NotifyFailure("Position not found.", "Position");

        var hasHeldPositionBefore = await _employeePositionsRepository.HasEmployeeEverHeldPosition(employeeId, request.PositionId);
        if (hasHeldPositionBefore) return NotifyFailure("Employee has already held this position before.", "Position");

        var dataOfChange = DateTime.UtcNow;
        
        var activePosition = await _employeePositionsRepository.GetActivePositionAsync(employeeId);
        if (activePosition != null)
        {
            activePosition.ClosePosition(dataOfChange);

            if (!activePosition.Validation())
            {
                return NotifyFailure<bool>(
                    errors: activePosition.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage))
                );
            }
        }
        
        var newPosition = new EmployeePosition(employee, position, dataOfChange);
        if (!newPosition.Validation())
        {
            return NotifyFailure<bool>(
                errors: newPosition.ValidationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage))
            );
        }

        await _employeePositionsRepository.AddAsync(newPosition);
        await _unitOfWork.CommitAsync();
        
        var response = _mapper.Map<ChangePositionResponse>(newPosition);
        return Result<ChangePositionResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyFailure("Employee not found.");
        
        _employeeRepository.Delete(employee);
        var success = await _unitOfWork.CommitAsync();
        
        return success ? Result.Success() : NotifyFailure("Database error while deleting employee.");
    }

    public async Task<Result<EmployeeResponse>> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return NotifyFailure<EmployeeResponse>("Employee not found.");

        var response = _mapper.Map<EmployeeResponse>(employee);
        return Result<EmployeeResponse>.Success(response);
    }

    public async Task<Result<EmployeeAndPositionsResponse>> GetHistoryAsync(int id)
    {
        var employee = await _employeeRepository.GetHistoryAsync(id);
        if (employee == null) return NotifyFailure<EmployeeAndPositionsResponse>("Employee not found.");
        
        var response = _mapper.Map<EmployeeAndPositionsResponse>(employee);
        return Result<EmployeeAndPositionsResponse>.Success(response);
    }
    
    public async Task<Result<PagedResponse<EmployeeResponse>>> SearchAsync(EmployeeFilter filter)
    {
        var validator = new EmployeeFilterValidator();
        var validationResult = await validator.ValidateAsync(filter);

        if (!validationResult.IsValid)
        {
            return NotifyFailure<PagedResponse<EmployeeResponse>>(
                errors: validationResult.Errors.Select(e => (e.PropertyName, e.ErrorMessage))
            );
        }

        var (employee, total) = await _employeeRepository.SearchAsync(filter);
        var mapperData = _mapper.Map<IEnumerable<EmployeeResponse>>(employee);

        var pagedDataResponse = new PagedResponse<EmployeeResponse>(mapperData, total, filter.PageNumber, filter.PageSize);
        return Result<PagedResponse<EmployeeResponse>>.Success(pagedDataResponse);
    }
}