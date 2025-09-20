// Finance.Domain/Entities/CostCenter.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.Entities
{
    /// <summary>
    /// A cost center to link expenses/restrictions to a department/department (linked to Users/HR via DepartmentId).
    /// </summary>
    public class CostCenter : BaseEntity<int>, IAggregateRoot
    {
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public int? DepartmentId { get; private set; } // From HR/Users (reference via Integration Link)
        public bool IsActive { get; private set; } = true;

        private CostCenter() { }
        public CostCenter(string code, string name, int? departmentId = null)
        {
            Code = code.Trim();
            Name = name.Trim();
            DepartmentId = departmentId;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
