using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Base;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Dto.Filters;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Services;

namespace OnboardingSIGDB1.API.Controller;

[ApiController]
[Route("[controller]")]
public class CompaniesController : ControllerBase
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
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CompanyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CompanyFilter filter)
    {
        var result = await _companyService.SearchAsync(filter);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _companyService.GetByIdAsync(id);
        
        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }
    
    [HttpGet("{id:int}/employees")]
    [ProducesResponseType(typeof(CompanyAndEmployeesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployees(int id)
    {
        var response = await _companyService.GetByIdCompanyAndEmployees(id);
        
        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CompanyRequest request)
    {
        var response = await _companyService.CreateAsync(request);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] CompanyRequest request)
    {
        var response = await _companyService.UpdateAsync(id, request);
        
        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);

        if (_notificationContext.HasNotifications)
            return BadRequest(_notificationContext.Notifications);
        
        return NoContent(); 
    }
}