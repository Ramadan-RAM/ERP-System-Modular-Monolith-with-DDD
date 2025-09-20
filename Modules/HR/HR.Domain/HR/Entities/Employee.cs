using ERPSys.SharedKernel.Domain;
using HR.Domain.Events;
using HR.Domain.Events.HR.Domain.Events;
using HR.Domain.HR.Entities;

public class Employee : Person
{
    public string EmployeeCode { get; set; } = string.Empty;

    public int JobTitleId { get; private set; }
    public JobTitle? JobTitle { get; private set; }

    public int? DepartmentId { get;  set; }
    public Department? Department { get; private set; }

    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary => BasicSalary + Allowances - Deductions;

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

    public List<EmployeeLeave> Leaves { get; set; } = new();
    public List<Payslip> Payslips { get; set; } = new();

    public void MarkAsCreated()
    {
        var fullName = $"{FirstName} {MiddelName} {LastName}";
        var employeeCreatedEvent = new EmployeeCreatedEvent(
            Id,
            DepartmentId ?? 0,            // ✅ We must send it
            Department?.Name ?? string.Empty,
            JobTitle?.Title ?? string.Empty,
            EmployeeCode,
            fullName,
            NetSalary
        );

        AddDomainEvent(employeeCreatedEvent);
    }




    public void AssignDepartmentAndJobTitle(Department department, JobTitle jobTitle)
    {
        if (department == null)
            throw new ArgumentNullException(nameof(department), "Department is required.");

        if (jobTitle == null)
            throw new ArgumentNullException(nameof(jobTitle), "JobTitle is required.");

        // If the given job title does not belong to the department, auto-correct it
        if (jobTitle.DepartmentId != department.Id)
        {
            // Try to find a valid job title in the same department
            var validJobTitle = department.JobTitles?.FirstOrDefault();
            if (validJobTitle != null)
            {
                jobTitle = validJobTitle;
            }
            else
            {
                throw new InvalidOperationException(
                    $"No JobTitles found for Department '{department.Name}'.");
            }
        }

        DepartmentId = department.Id;
        Department = department;
        JobTitleId = jobTitle.Id;
        JobTitle = jobTitle;
    }

}


