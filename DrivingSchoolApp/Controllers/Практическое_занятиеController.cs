using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Практическое_занятиеController : ControllerBase
{
    private readonly DataService _dataService;
    public Практическое_занятиеController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DrivingLessonDto>>> GetAll()
        => Ok(await _dataService.GetAllDrivingLessonsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<DrivingLessonDto>> GetById(int id)
    {
        var item = await _dataService.GetDrivingLessonByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DrivingLessonDto>> Create(DrivingLessonDto dto)
    {
        try
        {
            var created = await _dataService.CreateDrivingLessonAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DrivingLessonDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateDrivingLessonAsync(id, dto);
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
        var deleted = await _dataService.DeleteDrivingLessonAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}