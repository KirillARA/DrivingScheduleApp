using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ГруппаController : ControllerBase
{
    private readonly DataService _dataService;
    public ГруппаController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetAll()
        => Ok(await _dataService.GetAllGroupsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDto>> GetById(int id)
    {
        var item = await _dataService.GetGroupByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> Create(GroupDto dto)
    {
        try
        {
            var created = await _dataService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, GroupDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateGroupAsync(id, dto);
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
        var deleted = await _dataService.DeleteGroupAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}