using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Companies.Commands;
using OnboardingSIGDB1.Domain.Dto.Companies.Responses;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Common.Pagination;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CompaniesController : ApiBaseController
{
    private readonly ICompanyService _companyService;
    private readonly INotificationContext _notificationContext;
    
    public CompaniesController(
        ICompanyService companyService,
        INotificationContext notificationContext)
    {
        _notificationContext = notificationContext;
        _companyService = companyService;
    }
    
    /// <summary>
    /// Gets a paginated list of companies with optional filters.
    /// </summary>
    /// <param name="filter">Query filters (name, cnpj, foundation date range, and pagination).</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CompanyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] CompanyFilter filter)
    {
        if (!ModelState.IsValid)
        {
            AddModelStateNotifications(_notificationContext);
            return NotificationError(_notificationContext);
        }
        
        var result = await _companyService.SearchAsync(filter);
        
        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Gets a company by its identifier.
    /// </summary>
    /// <param name="id">Company identifier.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _companyService.GetByIdAsync(id);
        
        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);
        
        return Ok(response);
    }
    
    /// <summary>
    /// Gets a company by identifier including employee position history.
    /// </summary>
    /// <param name="id">Company identifier.</param>
    [HttpGet("{id:int}/employees")]
    [ProducesResponseType(typeof(CompanyAndEmployeesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEmployees(int id)
    {
        var response = await _companyService.GetCompanyWithEmployeesByIdAsync(id);
        
        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);
        
        return Ok(response);
    }

    /// <summary>
    /// Creates a new company.
    /// </summary>
    /// <param name="request">Company payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CompanyRequest? request)
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
        
        var response = await _companyService.CreateAsync(request);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);

        return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
    }

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="id">Company identifier.</param>
    /// <param name="request">Updated company payload.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] CompanyRequest? request)
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
        
        var response = await _companyService.UpdateAsync(id, request);
        
        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);
        
        return Ok(response);
    }

    /// <summary>
    /// Deletes a company by its identifier.
    /// </summary>
    /// <param name="id">Company identifier.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);

        if (!_notificationContext.IsValid)
            return NotificationError(_notificationContext);
        
        return NoContent(); 
    }
}