using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.filters.Validators;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Services;

namespace OnboardingSIGDB1.Domain.Services.Positions;

public class PositionService : IPositionService
{ 
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationContext _notificationContext;
    private readonly IMapper _mapper;
    
    public PositionService(
        IMapper mapper, 
        IPositionRepository positionRepository, 
        IUnitOfWork unitOfWork, 
        INotificationContext notificationContext)
    {
        _mapper  = mapper;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
        _notificationContext = notificationContext;
    }

    private PositionResponse? NotifyError(string key, string message)
    {
        _notificationContext.AddNotification(key, message);
        return null;
    }
    
    private PositionResponse? AddDomainNotifications(Position position)
    {
        _notificationContext.AddRange(position.ValidationResult.Errors);
        return null;
    }
    
    private bool NotifyErrorBool(string key, string message)
    {
        _notificationContext.AddNotification(key, message);
        return false;
    }
    
    public async Task<PositionResponse?> CreateAsync(PositionRequest request)
    {        
        var existingPosition = await _positionRepository.GetByDescriptionAsync(request.Description.Trim());
        if (existingPosition != null) return NotifyError("Description", "This position already exists.");
        
        var position = new Position(request.Description);

        if (!position.Validation()) return AddDomainNotifications(position);
        
        await _positionRepository.AddAsync(position);
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<PositionResponse>(position);
    }
    
    public async Task<PositionResponse?> UpdateAsync(int id, PositionRequest request)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotifyError("Position", "Position not found.");

        var descriptionClean = request.Description.Trim();
        var existingWithDescription = await _positionRepository.GetByDescriptionAsync(descriptionClean);
        if (existingWithDescription != null && existingWithDescription.Id != id) 
            return NotifyError("Description", "Another position already uses this description.");
        
        position.Update(request.Description);

        if (!position.Validation()) return AddDomainNotifications(position);
        
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<PositionResponse>(position);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotifyErrorBool("Position", "Position not found.");
        
        if (await _positionRepository.HasEmployeesAsync(position.Id)) 
            return NotifyErrorBool("Position", "Cannot delete a position linked to employees.");
        
        _positionRepository.Delete(position);
        
        return await _unitOfWork.CommitAsync();
    }

    public async Task<PositionResponse?> GetByIdAsync(int id)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotifyError("Position", "Position not found.");
        
        return _mapper.Map<PositionResponse>(position);
    }
    
    public async Task<PagedResponse<PositionResponse>> SearchAsync(PositionFilter filter)
    {
        var validator = new PositionFilterValidator();
        var validationResult = await validator.ValidateAsync(filter);
        
        if (!validationResult.IsValid)
        {
            _notificationContext.AddRange(validationResult.Errors);

            return new PagedResponse<PositionResponse>(
                Enumerable.Empty<PositionResponse>(),
                0,
                filter.PageNumber,
                filter.PageSize
            );
        }
        
        var (position, total) = await _positionRepository.SearchAsync(filter);
        
        var mapperData = _mapper.Map<IEnumerable<PositionResponse>>(position);
        
        return new PagedResponse<PositionResponse>(
            mapperData,
            total,
            filter.PageNumber,
            filter.PageSize
        );
    }
}