using AutoMapper;
using FluentValidation;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Services.Base;

namespace OnboardingSIGDB1.Domain.Services.Positions;

public class PositionService : BaseService, IPositionService 
{ 
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<PositionFilter> _positionFilterValidator;
    
    public PositionService(
        IMapper mapper, 
        IPositionRepository positionRepository, 
        IUnitOfWork unitOfWork, 
        INotificationContext notificationContext,
        IValidator<PositionFilter> positionFilterValidator)
        : base(notificationContext)
    {
        _mapper  = mapper;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
        _positionFilterValidator = positionFilterValidator;
    }
    
    public async Task<PositionResponse?> CreateAsync(PositionRequest request)
    {        
        var description = request.Description?.Trim() ?? string.Empty;
        var existingPosition = await _positionRepository.GetByDescriptionAsync(description);
        if (existingPosition != null) return NotifyError<PositionResponse>(nameof(Position.Description), "This position already exists.");

        var position = new Position(description);

        if (!position.Validation()) return AddDomainNotifications<PositionResponse>(position.ValidationResult);

        await _positionRepository.AddAsync(position);
        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyError<PositionResponse>("Commit", "Unable to save changes.");

        return _mapper.Map<PositionResponse>(position);
    }
    
    public async Task<PositionResponse?> UpdateAsync(int id, PositionRequest request)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotifyError<PositionResponse>("Position", "Position not found.");
        var description = request.Description?.Trim() ?? string.Empty;
        var existingWithDescription = await _positionRepository.GetByDescriptionAsync(description);
        if (existingWithDescription != null && existingWithDescription.Id != id)
            return NotifyError<PositionResponse>(nameof(Position.Description), "Another position already uses this description.");

        position.Update(description);

        if (!position.Validation()) return AddDomainNotifications<PositionResponse>(position.ValidationResult);

        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyError<PositionResponse>("Commit", "Unable to save changes.");

        return _mapper.Map<PositionResponse>(position);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotifyErrorBool("Position", "Position not found.");
        
        if (await _positionRepository.HasEmployeesAsync(position.Id)) 
            return NotifyErrorBool("Position", "Cannot delete a position linked to employees.");
        
        _positionRepository.Delete(position);
        
        var commitOk = await _unitOfWork.CommitAsync();
        if (!commitOk) return NotifyErrorBool("Commit", "Unable to save changes.");
        
        return true;
    }

    public async Task<PositionResponse?> GetByIdAsync(int id)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return NotifyError<PositionResponse>("Position", "Position not found.");
        
        return _mapper.Map<PositionResponse>(position);
    }
    
    public async Task<PagedResponse<PositionResponse>> SearchAsync(PositionFilter filter)
    {
        var validationResult = await _positionFilterValidator.ValidateAsync(filter);
        
        if (!validationResult.IsValid)
        {
            AddValidationErrors(validationResult);

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