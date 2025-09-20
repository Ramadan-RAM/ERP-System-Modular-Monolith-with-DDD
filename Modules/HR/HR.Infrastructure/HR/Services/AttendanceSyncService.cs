using HR.Application.Interfaces.Attendance;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.Services
{
    public class AttendanceSyncService
    {
        private readonly HRDbContext _context;
        private readonly IEnumerable<IAttendanceProvider> _providers;

        public AttendanceSyncService(HRDbContext context, IEnumerable<IAttendanceProvider> providers)
        {
            _context = context;
            _providers = providers;
        }

        public async Task SyncAllAsync()
        {
            var devices = await _context.FingerprintDevices.Include(d => d.Branch).ToListAsync();

            foreach (var device in devices)
            {
                var provider = _providers.FirstOrDefault(p => p.ProviderName == device.Type);
                if (provider == null) continue;

                var records = await provider.GetAttendanceAsync(device);
                await _context.AttendanceRecords.AddRangeAsync(records);
            }

            await _context.SaveChangesAsync();
        }
    }
}
