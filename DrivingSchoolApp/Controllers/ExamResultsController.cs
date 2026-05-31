using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/exam-results")]
[ApiController]
public class ExamResultsController : ControllerBase
{
    private readonly DataService _dataService;
    public ExamResultsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExamResultDto>>> GetAll(
        [FromQuery] string? student,
        [FromQuery] string? exam,
        [FromQuery] string? result,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        var items = await _dataService.GetAllExamResultsAsync(student, exam, result, dateFrom, dateTo);
        return Ok(items);
    }

    [HttpGet("{studentId}/{examId}/{date}")]
    public async Task<ActionResult<ExamResultDto>> GetById(int studentId, int examId, DateOnly date)
    {
        var item = await _dataService.GetExamResultByIdAsync(studentId, examId, date);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ExamResultDto>> Create(ExamResultDto dto)
    {
        try
        {
            var created = await _dataService.CreateExamResultAsync(dto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{studentId}/{examId}/{oldDate}")]
    public async Task<IActionResult> Update(int studentId, int examId, DateOnly oldDate, ExamResultDto dto)
    {
        try
        {
            var updated = await _dataService.UpdateExamResultAsync(studentId, examId, oldDate, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{studentId}/{examId}/{date}")]
    public async Task<IActionResult> Delete(int studentId, int examId, DateOnly date)
    {
        var deleted = await _dataService.DeleteExamResultAsync(studentId, examId, date);
        return deleted ? NoContent() : NotFound();
    }
}