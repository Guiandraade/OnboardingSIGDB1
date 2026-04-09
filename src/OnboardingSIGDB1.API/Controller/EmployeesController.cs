using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.EmployeeAndPositions.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;

namespace OnboardingSIGDB1.API.Controller;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly INotificationContext _notificationContext;
    
    public EmployeesController(IEmployeeService employeeService, INotificationContext notificationContext)
    {
        _employeeService = employeeService;
        _notificationContext = notificationContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EmployeeFilter filter)
    {
        var result = await _employeeService.SearchAsync(filter);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _employeeService.GetByIdAsync(id);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EmployeeRequest request)
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

        var result = await _employeeService.CreateAsync(request);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] EmployeeUpdateRequest request)
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

        var result = await _employeeService.UpdateAsync(id, request);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _employeeService.DeleteAsync(id);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return NoContent();
    }
    
    [HttpGet("{id:int}/positions")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var result = await _employeeService.GetHistoryAsync(id);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return Ok(result);
    }
    
    [HttpPost("{id:int}/positions")]
    public async Task<IActionResult> ChangePosition(int id, [FromBody] ChangeEmployeePositionRequest request)
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

        var result = await _employeeService.ChangePositionAsync(id, request);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);

        return Ok(result);
    }
}