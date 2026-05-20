using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Common.Pagination;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Commands;
using OnboardingSIGDB1.Domain.Dto.Positions.Responses;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService  _positionService;
    private readonly INotificationContext _notificationContext;
    
    public PositionsController(IPositionService positionService, INotificationContext notificationContext)
    {
        _positionService = positionService;
        _notificationContext = notificationContext;
    }

    /// <summary>
    /// Gets a paginated list of positions with optional filters.
    /// </summary>
    /// <param name="filter">Query filters (description and pagination).</param>
    [HttpGet] 
    [ProducesResponseType(typeof(PagedResponse<PositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] PositionFilter filter)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                _notificationContext.AddNotification("Model", error.ErrorMessage);
            return BadRequest(_notificationContext.Notifications);
        }
        
        var result = await _positionService.SearchAsync(filter);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Gets a position by identifier.
    /// </summary>
    /// <param name="id">Position identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PositionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _positionService.GetByIdAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    /// <summary>
    /// Creates a new position.
    /// </summary>
    /// <param name="request">Position payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(PositionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] PositionRequest? request)
    {
        if (request == null) 
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return BadRequest(_notificationContext.Notifications);
        }
    
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _notificationContext.AddNotification("Model", error.ErrorMessage);
            }
            return BadRequest(_notificationContext.Notifications);
        }
        
        var response = await _positionService.CreateAsync(request);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
    }

    /// <summary>
    /// Updates an existing position.
    /// </summary>
    /// <param name="id">Position identifier.</param>
    /// <param name="request">Updated position payload.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PositionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] PositionRequest? request)
    {
        if (request == null) 
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return BadRequest(_notificationContext.Notifications);
        }
    
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _notificationContext.AddNotification("Model", error.ErrorMessage);
            }
            return BadRequest(_notificationContext.Notifications);
        }
        
        var response = await _positionService.UpdateAsync(id, request);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response); 
    }

    /// <summary>
    /// Deletes a position by identifier.
    /// </summary>
    /// <param name="id">Position identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        await _positionService.DeleteAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return NoContent(); 
    }
}