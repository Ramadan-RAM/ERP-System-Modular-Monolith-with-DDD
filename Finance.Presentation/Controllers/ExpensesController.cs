using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Presentation.Controllers
{
    [ApiController]
    [Route("api/finance/expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpensesController(IExpenseService service)
        {
            _service = service;
        }

        // GET: api/finance/expenses/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GeneralExpenseDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // GET: api/finance/expenses
        // Supports optional filtering by period: ?start=2025-01-01&end=2025-01-31
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GeneralExpenseDto>>> GetAll([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            if (start.HasValue && end.HasValue)
            {
                var filtered = await _service.GetByPeriodAsync(start.Value, end.Value);
                return Ok(filtered);
            }

            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // POST: api/finance/expenses
        [HttpPost]
        public async Task<ActionResult<GeneralExpenseDto>> Create([FromBody] GeneralExpenseDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/finance/expenses/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GeneralExpenseDto dto)
        {
            if (id != dto.Id) return BadRequest("Mismatched id");

            await _service.UpdateAsync(dto);
            return NoContent();
        }

        // DELETE: api/finance/expenses/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/finance/expenses/by-department?departmentId=...&start=...&end=...
        [HttpGet("by-department")]
        public async Task<ActionResult<IReadOnlyList<GeneralExpenseDto>>> GetByDepartment(
            [FromQuery] Guid departmentId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var list = await _service.GetByDepartmentAsync(departmentId, start, end);
            return Ok(list);
        }

        // GET: api/finance/expenses/total-by-category?category=Utilities&start=...&end=...
        [HttpGet("total-by-category")]
        public async Task<ActionResult<object>> GetTotalByCategory(
            [FromQuery] string category,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var total = await _service.GetTotalByCategoryAsync(category, start, end);
            return Ok(new { category, start = start.Date, end = end.Date, total });
        }
    }
}
