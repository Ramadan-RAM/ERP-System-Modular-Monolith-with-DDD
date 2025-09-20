using ERPSys.SharedKernel.Domain;
using HR.Domain.Common;

namespace HR.Domain.HR.Entities
{
    public class Branch : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<FingerprintDevice> Devices { get; set; } = new List<FingerprintDevice>();
    }
}
