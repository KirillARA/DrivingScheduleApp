using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/discounts")]
[ApiController]
public class DiscountsController : ControllerBase
{
    private readonly DataService _dataService;
    public DiscountsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] int? minPercent,
        [FromQuery] int? maxPercent)
    {
        var items = await _dataService.GetAllDiscountsAsync(name, minPercent, maxPercent);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountDto>> GetById(int id)
    {
        var item = await _dataService.GetDiscountByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DiscountDto>> Create(DiscountDto dto)
    {
        try
        {
            var created = await _dataService.CreateDiscountAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DiscountDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateDiscountAsync(id, dto);
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
        var deleted = await _dataService.DeleteDiscountAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}