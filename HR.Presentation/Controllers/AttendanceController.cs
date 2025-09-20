using HR.Application.DTOs.HR;
using HR.Application.Interfaces.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Presentation.Controllers
{
    [ApiController]
    [Route("api/hr/[controller]")]
    
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");

        [HttpGet]
        public async Task<ActionResult<List<AttendanceRecordDTO>>> GetAll()
        {
            var records = await _attendanceService.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("by-employee/{employeeId}")]
        public async Task<ActionResult<List<AttendanceRecordDTO>>> GetByEmployeeId(int employeeId)
        {
            var records = await _attendanceService.GetByEmployeeIdAsync(employeeId);
            return Ok(records);
        }


        //[HttpPost("upload-excel")]
        //[Consumes("multipart/form-data")] // ✅ Very important for Swagger
        //public async Task<IActionResult> UploadExcel([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest(new { message = "No file uploaded or file is empty." });

        //    var result = await _attendanceService.ImportFromExcelAsync(file);
        //    return result
        //        ? Ok(new { message = "Excel uploaded successfully." })
        //        : StatusCode(500, new { message = "Failed to import Excel data." });
        //}



        [HttpPost("sync-devices")]
        public async Task<IActionResult> SyncFromDevices()
        {
            await _attendanceService.SyncFromDevicesAsync();
            return Ok(new { message = "Data synced from devices" });
        }
    }
}
