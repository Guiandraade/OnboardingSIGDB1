using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Common.Pagination;
using OnboardingSIGDB1.Domain.Dto.Employees.Responses;
using OnboardingSIGDB1.Domain.Dto.Employees.Commands;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class EmployeesController : ApiBaseController
{
    private readonly IEmployeeService _employeeService;
    private readonly INotificationContext _notificationContext;

    public EmployeesController(IEmployeeService employeeService, INotificationContext notificationContext)
    {
        _employeeService = employeeService;
        _notificationContext = notificationContext;
    }

    /// <summary>
    /// Gets a paginated list of employees with optional filters.
    /// </summary>
    /// <param name="filter">Query filters (name, cpf, hiring date range, and pagination).</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] EmployeeFilter filter)
    {
        if (!ModelState.IsValid)
        {
            AddModelStateNotifications(_notificationContext);
            return NotificationError(_notificationContext);
        }

        var result = await _employeeService.SearchAsync(filter);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return Ok(result);
    }

    /// <summary>
    /// Gets an employee by identifier.
    /// </summary>
    /// <param name="id">Employee identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _employeeService.GetByIdAsync(id);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return Ok(response);
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="request">Employee payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest? request)
    {
        if (request == null)
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return NotificationError(_notificationContext);
        }

        if (!ModelState.IsValid)
        {
            AddModelStateNotifications(_notificationContext);
            return NotificationError(_notificationContext);
        }

        var response = await _employeeService.CreateAsync(request);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">Employee identifier.</param>
    /// <param name="request">Updated employee payload.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateRequest? request)
    {
        if (request == null)
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return NotificationError(_notificationContext);
        }

        if (!ModelState.IsValid)
        {
            AddModelStateNotifications(_notificationContext);
            return NotificationError(_notificationContext);
        }

        var response = await _employeeService.UpdateAsync(id, request);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return Ok(response);
    }

    /// <summary>
    /// Deletes an employee by identifier.
    /// </summary>
    /// <param name="id">Employee identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return NoContent();
    }

    /// <summary>
    /// Gets an employee with full position history.
    /// </summary>
    /// <param name="id">Employee identifier.</param>
    [HttpGet("{id:int}/positions")]
    [ProducesResponseType(typeof(EmployeeAndPositionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var response = await _employeeService.GetHistoryAsync(id);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return Ok(response);
    }

    /// <summary>
    /// Changes the current position of an employee.
    /// </summary>
    /// <param name="id">Employee identifier.</param>
    /// <param name="request">Position change payload.</param>
    [HttpPost("{id:int}/positions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePosition(int id, [FromBody] ChangeEmployeePositionRequest? request)
    {
        if (request == null)
        {
            _notificationContext.AddNotification("Request", "Invalid data or empty request body.");
            return NotificationError(_notificationContext);
        }

        if (!ModelState.IsValid)
        {
            AddModelStateNotifications(_notificationContext);
            return NotificationError(_notificationContext);
        }

        await _employeeService.ChangePositionAsync(id, request);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return Ok();
    }
}