using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Presentation.Controllers
{
    [ApiController]
    [Route("api/hr/[controller]")]
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class PayslipsController : ControllerBase
    {
        private readonly IPayslipService _payslipService;

        public PayslipsController(IPayslipService payslipService)
        {
            _payslipService = payslipService;
        }


        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _payslipService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payslip = await _payslipService.GetByIdAsync(id);
            return payslip == null ? NotFound() : Ok(payslip);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PayslipDTO dto)
        {
            var result = await _payslipService.CreateAsync(dto);
            if (result.IsSuccess && result.Data != null)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
            }
            return BadRequest(new { error = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PayslipDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var result = await _payslipService.UpdateAsync(id, dto);
            if (result.IsSuccess && result.Data)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _payslipService.DeleteAsync(id);
            return result.IsSuccess && result.Data ? NoContent() : NotFound();
        }

        /// ✅ Generate Payslip Automatically
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromQuery] int employeeId, [FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var payslip = await _payslipService.GeneratePayslipAsync(employeeId, year, month);
                return Ok(payslip);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("generate-all")]
        public async Task<IActionResult> GeneratePayslipsForAll(int year, int month)
        {
            try
            {
                var result = await _payslipService.GeneratePayslipsForAllAsync(year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
