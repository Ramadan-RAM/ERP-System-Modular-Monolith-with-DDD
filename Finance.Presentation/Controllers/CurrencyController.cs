using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        // ✅ Get All
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CurrencyDto>>> GetAll()
        {
            var result = await _currencyService.GetAllAsync();
            return Ok(result);
        }

        // ✅ Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyDto>> GetById(int id)
        {
            var currency = await _currencyService.GetByIdAsync(id);
            if (currency == null) return NotFound();
            return Ok(currency);
        }

        // ✅ Create
        [HttpPost]
        public async Task<ActionResult<CurrencyDto>> Create([FromBody] CurrencyDto dto)
        {
            var created = await _currencyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CurrencyDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            await _currencyService.UpdateAsync(dto);
            return NoContent();
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _currencyService.DeleteAsync(id);
            return NoContent();
        }

        // ✅ Custom
        [HttpGet("code/{code}")]
        public async Task<ActionResult<CurrencyDto>> GetByCode(string code)
        {
            var currency = await _currencyService.GetByCodeAsync(code);
            if (currency == null) return NotFound();
            return Ok(currency);
        }

        [HttpGet("base")]
        public async Task<ActionResult<CurrencyDto>> GetBaseCurrency()
        {
            var currency = await _currencyService.GetBaseCurrencyAsync();
            if (currency == null) return NotFound();
            return Ok(currency);
        }
    }
}
