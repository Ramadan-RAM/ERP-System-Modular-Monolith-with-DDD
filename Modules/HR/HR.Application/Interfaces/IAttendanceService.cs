using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using Microsoft.AspNetCore.Http;

namespace HR.Application.Interfaces.HR  
{
    public interface IAttendanceService
    {
        Task<Result<List<AttendanceRecordDTO>>> GetAllAsync();
        Task<Result<List<AttendanceRecordDTO>>> GetByEmployeeIdAsync(int employeeId);
        Task<Result<bool>> ImportFromExcelAsync(IFormFile excelFile);
        Task<Result<bool>> SyncFromDevicesAsync();
    }

}
