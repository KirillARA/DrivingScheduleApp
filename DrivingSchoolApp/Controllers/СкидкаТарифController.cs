using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrivingSchoolApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class СкидкаТарифController : ControllerBase
{
    private readonly DataService _dataService;
    public СкидкаТарифController(DataService dataService) => _dataService = dataService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountTariffDto>>> GetAll()
        => Ok(await _dataService.GetAllDiscountTariffsAsync());

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

    
    [HttpDelete("{discountId}/{tariffId}")]
    public async Task<IActionResult> Delete(int discountId, int tariffId)
    {
        var deleted = await _dataService.DeleteDiscountTariffAsync(discountId, tariffId);
        return deleted ? NoContent() : NotFound();
    }
}