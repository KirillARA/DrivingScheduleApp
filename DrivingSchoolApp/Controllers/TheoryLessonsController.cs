using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/theory-lessons")]
[ApiController]
public class TheoryLessonsController : ControllerBase
{
    private readonly DataService _dataService;
    public TheoryLessonsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TheoryLessonDto>>> GetAll(
        [FromQuery] string? group,
        [FromQuery] string? teacher,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        var items = await _dataService.GetAllTheoryLessonsAsync(group, teacher, dateFrom, dateTo);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TheoryLessonDto>> GetById(int id)
    {
        var item = await _dataService.GetTheoryLessonByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TheoryLessonDto>> Create(TheoryLessonDto dto)
    {
        try
        {
            var created = await _dataService.CreateTheoryLessonAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TheoryLessonDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateTheoryLessonAsync(id, dto);
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
        var deleted = await _dataService.DeleteTheoryLessonAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}