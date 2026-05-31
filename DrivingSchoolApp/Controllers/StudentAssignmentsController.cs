using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/student-assignments")]
[ApiController]
public class StudentAssignmentsController : ControllerBase
{
    private readonly DataService _dataService;
    public StudentAssignmentsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentAssignmentDto>>> GetAll(
        [FromQuery] string? student,
        [FromQuery] string? instructor,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        var items = await _dataService.GetAllStudentAssignmentsAsync(student, instructor, dateFrom, dateTo);
        return Ok(items);
    }

    [HttpGet("{studentId}/{employeeId}/{assignmentDate}")]
    public async Task<ActionResult<StudentAssignmentDto>> GetById(int studentId, int employeeId, DateOnly assignmentDate)
    {
        var item = await _dataService.GetStudentAssignmentByIdAsync(studentId, employeeId, assignmentDate);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<StudentAssignmentDto>> Create(StudentAssignmentDto dto)
    {
        try
        {
            var created = await _dataService.CreateStudentAssignmentAsync(dto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{studentId}/{employeeId}/{assignmentDate}")]
    public async Task<IActionResult> Update(int studentId, int employeeId, DateOnly assignmentDate, StudentAssignmentDto dto)
    {
        try
        {
            var updated = await _dataService.UpdateStudentAssignmentAsync(studentId, employeeId, assignmentDate, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{studentId}/{employeeId}/{assignmentDate}")]
    public async Task<IActionResult> Delete(int studentId, int employeeId, DateOnly assignmentDate)
    {
        var deleted = await _dataService.DeleteStudentAssignmentAsync(studentId, employeeId, assignmentDate);
        return deleted ? NoContent() : NotFound();
    }
}