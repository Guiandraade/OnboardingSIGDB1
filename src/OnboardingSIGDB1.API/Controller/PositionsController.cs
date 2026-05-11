using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;

namespace OnboardingSIGDB1.API.Controller;

[ApiController]
[Route("[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService  _positionService;
    private readonly INotificationContext _notificationContext;
    
    public PositionsController(IPositionService positionService, INotificationContext notificationContext)
    {
        _positionService = positionService;
        _notificationContext = notificationContext;
    }

    [HttpGet] 
    [ProducesResponseType(typeof(PagedResponse<PositionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PositionFilter filter)
    {
        var result = await _positionService.SearchAsync(filter);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _positionService.GetByIdAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PositionRequest? request)
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] PositionRequest? request)
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _positionService.DeleteAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return NoContent(); 
    }
}