using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journalService;

        public JournalController(IJournalService journalService)
        {
            _journalService = journalService;
        }

        // ✅ Get All
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<JournalEntryDto>>> GetAll()
        {
            var result = await _journalService.GetAllAsync();
            return Ok(result);
        }

        // ✅ Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<JournalEntryDto>> GetById(int id)
        {
            var entry = await _journalService.GetByIdAsync(id);
            if (entry == null) return NotFound();
            return Ok(entry);
        }

        // ✅ Create
        [HttpPost]
        public async Task<ActionResult<JournalEntryDto>> Create([FromBody] JournalEntryDto dto)
        {
            var created = await _journalService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] JournalEntryDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");

            await _journalService.UpdateAsync(dto);
            return NoContent();
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _journalService.DeleteAsync(id);
            return NoContent();
        }

        // ✅ Approve
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _journalService.ApproveAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Journal entry approved successfully" });
        }

        // ✅ Post
        [HttpPost("{id}/post")]
        public async Task<IActionResult> Post(int id)
        {
            var success = await _journalService.PostAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Journal entry posted successfully" });
        }

        // ✅ Get By Date Range
        [HttpGet("range")]
        public async Task<ActionResult<IReadOnlyList<JournalEntryDto>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var entries = await _journalService.GetByDateRangeAsync(start, end);
            return Ok(entries);
        }
    }
}
