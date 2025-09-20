// File: API/Controllers/EmployeeLeaveController.cs

using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.API.Controllers.HR
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class EmployeeLeaveController : ControllerBase
    {
        private readonly IEmployeeLeaveService _service;

        public EmployeeLeaveController(IEmployeeLeaveService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeLeaveDTO dto)
        {
            var createdResult = await _service.CreateAsync(dto);
            if (!createdResult.IsSuccess || createdResult.Data == null)
                return BadRequest(createdResult.Message);

            return CreatedAtAction(nameof(GetById), new { id = createdResult.Data.Id }, createdResult.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EmployeeLeaveDTO dto)
        {
            var updatedResult = await _service.UpdateAsync(id, dto);
            if (!updatedResult.IsSuccess)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedResult = await _service.DeleteAsync(id);
            if (!deletedResult.IsSuccess || deletedResult.Data == null || !deletedResult.Data)
                return NotFound();
            return NoContent();
        }
    }
}
