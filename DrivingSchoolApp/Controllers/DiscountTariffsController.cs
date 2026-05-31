using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/discount-tariffs")]
[ApiController]
public class DiscountTariffsController : ControllerBase
{
    private readonly DataService _dataService;
    public DiscountTariffsController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountTariffDto>>> GetAll(
        [FromQuery] string? discount,
        [FromQuery] string? tariff,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo)
    {
        var items = await _dataService.GetAllDiscountTariffsAsync(discount, tariff, dateFrom, dateTo);
        return Ok(items);
    }

    [HttpGet("{discountId}/{tariffId}")]
    public async Task<ActionResult<DiscountTariffDto>> GetById(int discountId, int tariffId)
    {
        var item = await _dataService.GetDiscountTariffByIdAsync(discountId, tariffId);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DiscountTariffDto>> Create(DiscountTariffDto dto)
    {
        try
        {
            var created = await _dataService.CreateDiscountTariffAsync(dto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{discountId}/{tariffId}")]
    public async Task<IActionResult> Update(int discountId, int tariffId, DiscountTariffDto dto)
    {
        try
        {
            var updated = await _dataService.UpdateDiscountTariffAsync(discountId, tariffId, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpDelete("{discountId}/{tariffId}")]
    public async Task<IActionResult> Delete(int discountId, int tariffId)
    {
        var deleted = await _dataService.DeleteDiscountTariffAsync(discountId, tariffId);
        return deleted ? NoContent() : NotFound();
    }
}