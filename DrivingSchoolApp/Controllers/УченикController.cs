using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class УченикController : ControllerBase
{
    private readonly DataService _dataService;
    public УченикController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        => Ok(await _dataService.GetAllStudentsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        var item = await _dataService.GetStudentByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(StudentDto dto)
    {
        try
        {
            var created = await _dataService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StudentDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateStudentAsync(id, dto);
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
        var deleted = await _dataService.DeleteStudentAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}