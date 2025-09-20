namespace HR.Application.DTOs.HR;
public class AttendanceRecordDTO
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string? EmployeeFullName { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan? TimeIn { get; set; }

    public TimeSpan? TimeOut { get; set; }

    public string? DeviceId { get; set; }

    public string? Source { get; set; }
}