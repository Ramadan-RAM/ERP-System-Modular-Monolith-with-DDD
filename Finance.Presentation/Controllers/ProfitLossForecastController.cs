using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfitLossForecastController : ControllerBase
    {
        private readonly IProfitLossForecastService _service;

        public ProfitLossForecastController(IProfitLossForecastService service)
        {
            _service = service;
        }

        // ✅ Get All
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProfitLossForecastDto>>> GetAll()
        {
            var forecasts = await _service.GetAllAsync();
            return Ok(forecasts);
        }

        // ✅ Get By Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProfitLossForecastDto>> GetById(int id)
        {
            var forecast = await _service.GetByIdAsync(id);
            if (forecast == null) return NotFound();
            return Ok(forecast);
        }

        // ✅ Create
        [HttpPost]
        public async Task<ActionResult<ProfitLossForecastDto>> Create([FromBody] ProfitLossForecastDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ Update
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProfitLossForecastDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        // ✅ Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // ✅ Custom: Generate Forecast (Instead of the Dummy, we will link it to the InventorySnapshots later)
        [HttpPost("generate")]
        public async Task<ActionResult<ProfitLossForecastDto>> Generate([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var forecast = await _service.GenerateForecastAsync(start, end);
            return Ok(forecast);
        }

        // ✅ Custom: Get By Period
        [HttpGet("period")]
        public async Task<ActionResult<IReadOnlyList<ProfitLossForecastDto>>> GetByPeriod([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var forecasts = await _service.GetByPeriodAsync(start, end);
            return Ok(forecasts);
        }
    }
}
