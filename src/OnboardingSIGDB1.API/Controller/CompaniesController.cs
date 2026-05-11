using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
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
    public async Task<IActionResult> GetAll([FromQuery] CompanyFilter filter)
    {
        var result = await _companyService.SearchAsync(filter);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _companyService.GetByIdAsync(id);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }
    
    [HttpGet("{id:int}/employees")]
    public async Task<IActionResult> GetEmployees(int id)
    {
        var response = await _companyService.GetByIdCompanyAndEmployees(id);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CompanyRequest? request)
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
        
        var response = await _companyService.CreateAsync(request);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);

        if (response is null)
            return StatusCode(StatusCodes.Status500InternalServerError);
        
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] CompanyRequest? request)
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
        
        var response = await _companyService.UpdateAsync(id, request);
        
        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _companyService.DeleteAsync(id);

        if (!_notificationContext.IsValid)
            return BadRequest(_notificationContext.Notifications);
        
        return NoContent(); 
    }
}