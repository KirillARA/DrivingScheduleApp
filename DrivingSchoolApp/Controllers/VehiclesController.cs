using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/vehicles")]
[ApiController]
public class VehiclesController : ControllerBase
{
    private readonly DataService _dataService;
    public VehiclesController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransportDto>>> GetAll(
        [FromQuery] string? mark,
        [FromQuery] string? model,
        [FromQuery] string? category,
        [FromQuery] string? transmission)
    {
        var items = await _dataService.GetAllVehiclesAsync(mark, model, category, transmission);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransportDto>> GetById(int id)
    {
        var item = await _dataService.GetVehicleByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TransportDto>> Create(TransportDto dto)
    {
        try
        {
            var created = await _dataService.CreateVehicleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TransportDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        try
        {
            var updated = await _dataService.UpdateVehicleAsync(id, dto);
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
        var deleted = await _dataService.DeleteVehicleAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}