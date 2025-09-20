using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Presentation.Controllers
{

    [ApiController]
    [Route("api/hr/[controller]")]
    
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class FingerprintDevicesController : ControllerBase
    {
        private readonly IFingerprintDeviceService _deviceService;

        public FingerprintDevicesController(IFingerprintDeviceService deviceService)
        {
            _deviceService = deviceService;
        }


        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");


        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _deviceService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _deviceService.GetByIdAsync(id);
            return device == null ? NotFound() : Ok(device);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FingerprintDeviceDTO dto) =>
            Ok(await _deviceService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, FingerprintDeviceDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            var result = await _deviceService.UpdateAsync(id, dto);
            return result.IsSuccess ? NoContent() : NotFound(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _deviceService.DeleteAsync(id);
            return result.IsSuccess && result.Data ? NoContent() : NotFound(result.Message);
        }
    }
}
