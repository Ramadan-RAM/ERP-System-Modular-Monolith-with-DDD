
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.API.Controllers.HR
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class JobTitlesController : ControllerBase
    {
        private readonly IJobTitleService _jobTitleService;

        public JobTitlesController(IJobTitleService jobTitleService)
        {
            _jobTitleService = jobTitleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _jobTitleService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobTitleService.GetByIdAsync(id);
            return job == null ? NotFound() : Ok(job);
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobTitleDTO dto) =>
            Ok(await _jobTitleService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, JobTitleDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var result = await _jobTitleService.UpdateAsync(id, dto);
            return result.IsSuccess ? NoContent() : NotFound(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _jobTitleService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : NotFound(result.Message);
        }
    }
}
