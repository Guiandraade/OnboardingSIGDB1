using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;

namespace OnboardingSIGDB1.API.Controller;

[ApiController]
[Route("[controller]")]
public class EmpoyeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly INotificationContext _notificationContext;
    
    public EmpoyeesController(IEmployeeService employeeService, INotificationContext notificationContext)
    {
        _employeeService = employeeService;
        _notificationContext = notificationContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EmployeeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] EmployeeFilter filter)
    {
        var result = await _employeeService.SearchAsync(filter);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _employeeService.GetByIdAsync(id);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpGet("{id:int}/positions")]
    [ProducesResponseType(typeof(IEnumerable<EmployeeAndPositionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var history = await _employeeService.GetHistoryAsync(id);
    
        if (_notificationContext.HasNotifications) 
            return BadRequest(_notificationContext.Notifications);
    
        return Ok(history);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] EmployeeRequest request)
    {
        var response = await _employeeService.CreateAsync(request);

        if (_notificationContext.HasNotifications) 
            return BadRequest(_notificationContext.Notifications);
        
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(int id, [FromBody] EmployeeUpdateRequest request)
    {
        var response = await _employeeService.UpdateAsync(id, request);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return NoContent();
    }
}