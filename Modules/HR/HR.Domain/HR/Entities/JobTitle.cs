

namespace HR.Domain.HR.Entities
{
    public class JobTitle
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BaseSalary { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual Department? Department { get; set; } 
    }

}
