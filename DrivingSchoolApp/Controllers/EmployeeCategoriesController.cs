using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/employee-categories")]
[ApiController]
public class EmployeeCategoriesController : ControllerBase
{
    private readonly DataService _dataService;

    public EmployeeCategoriesController(DataService dataService)
        => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeCategoryDto>>> GetAll(
        [FromQuery] string? employee,
        [FromQuery] string? category)
    {
        var items = await _dataService.GetAllEmployeeCategoriesAsync(employee, category);
        return Ok(items);
    }

    [HttpGet("{employeeId}/{categoryId}")]
    public async Task<ActionResult<EmployeeCategoryDto>> GetById(int employeeId, int categoryId)
    {
        var item = await _dataService.GetEmployeeCategoryByIdAsync(employeeId, categoryId);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeCategoryDto>> Create(EmployeeCategoryDto dto)
    {
        try
        {
            var created = await _dataService.CreateEmployeeCategoryAsync(dto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{employeeId}/{categoryId}")]
    public async Task<IActionResult> Delete(int employeeId, int categoryId)
    {
        var deleted = await _dataService.DeleteEmployeeCategoryAsync(employeeId, categoryId);
        return deleted ? NoContent() : NotFound();
    }
}