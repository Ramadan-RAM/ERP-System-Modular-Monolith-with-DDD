using ERPSys.SharedKernel.Domain;

namespace HR.Domain.HR.Entities
{
    public class FingerprintDevice : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Type { get; set; } = "ZKTeco"; // Or Excel for example

        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
