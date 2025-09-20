using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventorySnapshotsController : ControllerBase
    {
        private readonly IInventorySnapshotService _service;

        public InventorySnapshotsController(IInventorySnapshotService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventorySnapshotDto>> GetById(int id)
        {
            var snapshot = await _service.GetByIdAsync(id);
            if (snapshot == null) return NotFound();
            return Ok(snapshot);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<InventorySnapshotDto>>> GetAll()
        {
            var snapshots = await _service.GetAllAsync();
            return Ok(snapshots);
        }

        [HttpPost]
        public async Task<ActionResult<InventorySnapshotDto>> Create([FromBody] InventorySnapshotDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventorySnapshotDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // ✅ Custom endpoints
        [HttpGet("near-expiry/{thresholdDays}")]
        public async Task<ActionResult<IReadOnlyList<InventorySnapshotDto>>> GetNearExpiry(int thresholdDays)
        {
            var items = await _service.GetNearExpiryAsync(thresholdDays);
            return Ok(items);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<ActionResult<IReadOnlyList<InventorySnapshotDto>>> GetByProduct(Guid productId)
        {
            var items = await _service.GetByProductAsync(productId);
            return Ok(items);
        }

        [HttpGet("by-period")]
        public async Task<ActionResult<IReadOnlyList<InventorySnapshotDto>>> GetByPeriod(DateTime start, DateTime end)
        {
            var items = await _service.GetByPeriodAsync(start, end);
            return Ok(items);
        }
    }
}
