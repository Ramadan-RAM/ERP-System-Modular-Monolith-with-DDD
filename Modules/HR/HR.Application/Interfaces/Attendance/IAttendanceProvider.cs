using HR.Domain.HR.Entities;

namespace HR.Application.Interfaces.Attendance
{

    public interface IAttendanceProvider
    {
        Task<List<AttendanceRecord>> GetAttendanceAsync(FingerprintDevice device);
        string ProviderName { get; }
    }
}
