using AutoMapper;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;

using HR.Application.Interfaces.HR;

using HR.Infrastructure.HR.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; 


namespace HR.Infrastructure.HR.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly HRDbContext _context;
        private readonly IMapper _mapper;
        private readonly AttendanceSyncService _syncService;

        public AttendanceService(HRDbContext context, IMapper mapper, AttendanceSyncService syncService)
        {
            _context = context;
            _mapper = mapper;
            _syncService = syncService;
        }

        public async Task<Result<List<AttendanceRecordDTO>>> GetAllAsync()
        {
            try
            {
                var records = await _context.AttendanceRecords
                    .Include(x => x.Employee)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<AttendanceRecordDTO>>(records);
                return Result<List<AttendanceRecordDTO>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return Result<List<AttendanceRecordDTO>>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<AttendanceRecordDTO>>> GetByEmployeeIdAsync(int employeeId)
        {
            try
            {
                var records = await _context.AttendanceRecords
                    .Where(r => r.EmployeeId == employeeId)
                    .ToListAsync();

                var dtoList = _mapper.Map<List<AttendanceRecordDTO>>(records);
                return Result<List<AttendanceRecordDTO>>.Success(dtoList);
            }
            catch (Exception ex)
            {
                return Result<List<AttendanceRecordDTO>>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> ImportFromExcelAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Result<bool>.Failure("File is empty");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                var records = new List<AttendanceRecordDTO>();

                for (int row = 2; row <= rowCount; row++)
                {
                    var empCode = worksheet.Cells[row, 1].Text.Trim();
                    var date = DateTime.Parse(worksheet.Cells[row, 2].Text.Trim());
                    var timeIn = TimeSpan.TryParse(worksheet.Cells[row, 3].Text.Trim(), out var ti) ? ti : (TimeSpan?)null;
                    var timeOut = TimeSpan.TryParse(worksheet.Cells[row, 4].Text.Trim(), out var to) ? to : (TimeSpan?)null;

                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == empCode && !e.IsDeleted);
                    if (employee == null) continue;

                    records.Add(new AttendanceRecordDTO
                    {
                        EmployeeId = employee.Id,
                        Date = date,
                        TimeIn = timeIn,
                        TimeOut = timeOut,
                        Source = "Excel"
                    });
                }

                // You may need to map DTOs to Entities here
                var entityRecords = _mapper.Map<List<Domain.HR.Entities.AttendanceRecord>>(records);

                await _context.AttendanceRecords.AddRangeAsync(entityRecords);
                await _context.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SyncFromDevicesAsync()
        {
            try
            {
                await _syncService.SyncAllAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
