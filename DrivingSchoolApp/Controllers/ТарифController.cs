using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ТарифController : ControllerBase
{
    private readonly DataService _dataService;
    public ТарифController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TariffDto>>> GetAll()
        => Ok(await _dataService.GetAllTariffsAsync());

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