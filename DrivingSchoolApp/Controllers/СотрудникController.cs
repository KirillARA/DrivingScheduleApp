using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class СотрудникController : ControllerBase
{
    private readonly DataService _dataService;
    public СотрудникController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        => Ok(await _dataService.GetAllEmployeesAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var item = await _dataService.GetEmployeeByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeDto dto)
    {
        try
        {
            var created = await _dataService.CreateEmployeeAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EmployeeDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateEmployeeAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _dataService.DeleteEmployeeAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}