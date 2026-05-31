using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/tariffs")]
[ApiController]
public class TariffsController : ControllerBase
{
    private readonly DataService _dataService;
    public TariffsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TariffDto>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? minHours,
        [FromQuery] int? maxHours,
        [FromQuery] string? category,
        [FromQuery] string? transmission)
    {
        var items = await _dataService.GetAllTariffsAsync(name, minPrice, maxPrice, minHours, maxHours, category, transmission);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TariffDto>> GetById(int id)
    {
        var item = await _dataService.GetTariffByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TariffDto>> Create(TariffDto dto)
    {
        try
        {
            var created = await _dataService.CreateTariffAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TariffDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateTariffAsync(id, dto);
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
        var deleted = await _dataService.DeleteTariffAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}