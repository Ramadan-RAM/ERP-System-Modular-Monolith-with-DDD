using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Finance.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GLAccountController : ControllerBase
    {
        private readonly IGLAccountService _service;

        public GLAccountController(IGLAccountService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GLAccountDto>> GetById(int id)
        {
            var account = await _service.GetByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GLAccountDto>>> GetAll()
        {
            var accounts = await _service.GetAllAsync();
            return Ok(accounts);
        }

        [HttpPost]
        public async Task<ActionResult<GLAccountDto>> Create(GLAccountDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, GLAccountDto dto)
        {
            if (id != dto.Id) return BadRequest("Mismatched ID");
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // 🔹 Custom
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<GLAccountDto?>> GetByCode(string code)
        {
            var account = await _service.GetByCodeAsync(code);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<IReadOnlyList<GLAccountDto>>> GetByType(string type)
        {
            var accounts = await _service.GetByTypeAsync(type);
            return Ok(accounts);
        }
    }
}
