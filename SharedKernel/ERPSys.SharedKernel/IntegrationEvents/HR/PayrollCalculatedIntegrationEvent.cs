using ERPSys.SharedKernel.Events;

public class PayrollCalculatedIntegrationEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public int EmployeeId { get; }
    public int DepartmentId { get; }
    public string EmployeeCode { get; }
    public string FullName { get; }
    public decimal NetSalary { get; }
    public string Currency { get; }
    public string Period { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PayrollCalculatedIntegrationEvent(
        int employeeId,
        int departmentId,
        string employeeCode,
        string fullName,
        decimal netSalary,
        string currency,
        string period)
    {
        EmployeeId = employeeId;
        DepartmentId = departmentId;
        EmployeeCode = employeeCode;
        FullName = fullName;
        NetSalary = netSalary;
        Currency = currency;   
        Period = period;
    }
}
