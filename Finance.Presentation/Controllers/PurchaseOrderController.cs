using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Finance.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        // ✅ Get All
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PurchaseOrderDto>>> GetAll()
        {
            var result = await _purchaseService.GetAllAsync();
            return Ok(result);
        }

        // ✅ Get By Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PurchaseOrderDto>> GetById(int id)
        {
            var result = await _purchaseService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ✅ Create
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] PurchaseOrderDto dto)
        {
            var result = await _purchaseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // ✅ Update
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseOrderDto dto)
        {
            if (id != dto.Id) return BadRequest("Mismatched Id");

            await _purchaseService.UpdateAsync(dto);
            return NoContent();
        }

        // ✅ Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _purchaseService.DeleteAsync(id);
            return NoContent();
        }

        // ✅ Add Item to Order
        [HttpPost("{orderId:int}/items")]
        public async Task<IActionResult> AddItem(int orderId, [FromBody] PurchaseItemDto itemDto)
        {
            await _purchaseService.AddItemAsync(orderId, itemDto);
            return NoContent();
        }

        // ✅ Get Orders By Supplier
        [HttpGet("supplier/{supplierId:guid}")]
        public async Task<ActionResult<IReadOnlyList<PurchaseOrderDto>>> GetBySupplier(Guid supplierId)
        {
            var result = await _purchaseService.GetBySupplierAsync(supplierId);
            return Ok(result);
        }

        // ✅ Get Orders By Date Range
        [HttpGet("daterange")]
        public async Task<ActionResult<IReadOnlyList<PurchaseOrderDto>>> GetByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _purchaseService.GetByDateRangeAsync(start, end);
            return Ok(result);
        }

        // ✅ Get Orders By Status
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IReadOnlyList<PurchaseOrderDto>>> GetByStatus(PurchaseStatus status)
        {
            var result = await _purchaseService.GetByStatusAsync(status);
            return Ok(result);
        }

        // ✅ Get Total Orders Amount
        [HttpGet("total/{supplierId:guid}")]
        public async Task<ActionResult<decimal>> GetTotalOrdersAmount(Guid supplierId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _purchaseService.GetTotalOrdersAmountAsync(supplierId, start, end);
            return Ok(result);
        }
    }
}
