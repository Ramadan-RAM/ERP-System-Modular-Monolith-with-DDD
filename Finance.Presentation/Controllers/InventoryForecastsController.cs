using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryForecastsController : ControllerBase
    {
        private readonly IInventoryForecastService _service;

        public InventoryForecastsController(IInventoryForecastService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<ProfitLossForecastDto>> Generate([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var forecast = await _service.GenerateForecastAsync(start, end);
            return Ok(forecast);
        }

        [HttpGet("near-expiry/{thresholdDays}")]
        public async Task<ActionResult<IReadOnlyList<InventoryForecastDto>>> GetNearExpiry(int thresholdDays)
        {
            var items = await _service.GetNearExpiryAsync(thresholdDays);
            return Ok(items);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IReadOnlyList<InventoryForecastDto>>> GetByStatus(ProfitLossStatus status)
        {
            var items = await _service.GetByStatusAsync(status);
            return Ok(items);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IReadOnlyList<InventoryForecastDto>>> GetByProduct(Guid productId)
        {
            var items = await _service.GetByProductAsync(productId);
            return Ok(items);
        }

        [HttpGet("by-period")]
        public async Task<ActionResult<IReadOnlyList<InventoryForecastDto>>> GetByPeriod(DateTime start, DateTime end)
        {
            var items = await _service.GetByPeriodAsync(start, end);
            return Ok(items);
        }
    }
}
