using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;

namespace OnboardingSIGDB1.API.Controllers;

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

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _employeeService.GetByIdAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest? request)
    {
        if (request == null)
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return BadRequest(_notificationContext.Notifications);
        }

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                _notificationContext.AddNotification("Model", error.ErrorMessage);
            return BadRequest(_notificationContext.Notifications);
        }

        var response = await _employeeService.CreateAsync(request);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateRequest? request)
    {
        if (request == null)
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return BadRequest(_notificationContext.Notifications);
        }

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                _notificationContext.AddNotification("Model", error.ErrorMessage);
            return BadRequest(_notificationContext.Notifications);
        }

        var response = await _employeeService.UpdateAsync(id, request);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return NoContent();
    }

    [HttpGet("{id:int}/positions")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var response = await _employeeService.GetHistoryAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return Ok(response);
    }

    [HttpPost("{id:int}/positions")]
    public async Task<IActionResult> ChangePosition(int id, [FromBody] ChangeEmployeePositionRequest? request)
    {
        if (request == null)
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return BadRequest(_notificationContext.Notifications);
        }

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                _notificationContext.AddNotification("Model", error.ErrorMessage);
            return BadRequest(_notificationContext.Notifications);
        }

        await _employeeService.ChangePositionAsync(id, request);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        return Ok();
    }
}