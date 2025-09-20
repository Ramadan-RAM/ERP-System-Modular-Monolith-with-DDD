using HR.Application.DTOs.HR;
using HR.Application.Interfaces.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Presentation.Controllers
{
    [ApiController]
    [Route("api/hr/[controller]")]
    
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");


        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _departmentService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);
            return dept == null ? NotFound() : Ok(dept);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDTO dto) =>
            Ok(await _departmentService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DepartmentDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var result = await _departmentService.UpdateAsync(id, dto);
            return result.IsSuccess ? NoContent() : NotFound(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : NotFound(result.Message);
        }
    }
}
