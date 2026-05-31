using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/exams")]
[ApiController]
public class ExamsController : ControllerBase
{
    private readonly DataService _dataService;
    public ExamsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExamDto>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? type,
        [FromQuery] string? transmission)
    {
        var items = await _dataService.GetAllExamsAsync(category, type, transmission);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExamDto>> GetById(int id)
    {
        var item = await _dataService.GetExamByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ExamDto>> Create(ExamDto dto)
    {
        try
        {
            var created = await _dataService.CreateExamAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ExamDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateExamAsync(id, dto);
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
        var deleted = await _dataService.DeleteExamAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}