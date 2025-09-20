using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        // ✅ Get All
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetAll()
        {
            var result = await _exchangeRateService.GetAllAsync();
            return Ok(result);
        }

        // ✅ Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ExchangeRateDto>> GetById(int id)
        {
            var rate = await _exchangeRateService.GetByIdAsync(id);
            if (rate == null) return NotFound();
            return Ok(rate);
        }

        // ✅ Create
        [HttpPost]
        public async Task<ActionResult<ExchangeRateDto>> Create([FromBody] ExchangeRateDto dto)
        {
            var created = await _exchangeRateService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExchangeRateDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            await _exchangeRateService.UpdateAsync(dto);
            return NoContent();
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _exchangeRateService.DeleteAsync(id);
            return NoContent();
        }

        // ✅ Custom
        [HttpGet("rate")]
        public async Task<ActionResult<ExchangeRateDto>> GetRate(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime date)
        {
            var rate = await _exchangeRateService.GetRateAsync(from, to, date);
            if (rate == null) return NotFound();
            return Ok(rate);
        }

        [HttpGet("latest")]
        public async Task<ActionResult<ExchangeRateDto>> GetLatestRate(
            [FromQuery] string from,
            [FromQuery] string to)
        {
            var rate = await _exchangeRateService.GetLatestRateAsync(from, to);
            if (rate == null) return NotFound();
            return Ok(rate);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetHistory(
            [FromQuery] string from,
            [FromQuery] string to)
        {
            var history = await _exchangeRateService.GetHistoryAsync(from, to);
            return Ok(history);
        }

        [HttpGet("by-date")]
        public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetByDate(
            [FromQuery] DateTime date)
        {
            var rates = await _exchangeRateService.GetByDateAsync(date);
            return Ok(rates);
        }

        [HttpGet("between")]
        public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetBetweenDates(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var rates = await _exchangeRateService.GetBetweenDatesAsync(from, to, start, end);
            return Ok(rates);
        }
    }
}
