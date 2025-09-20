using ERPSys.SharedKernel.Domain;
using HR.Domain.HR.Entities;

public class Department : BaseEntity<int>
{
   
    public string Name { get; set; }

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public bool IsDeleted { get; set; } = false;

    // ✅ Relationships
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<JobTitle> JobTitles { get; set; } = new List<JobTitle>(); // Added
}
