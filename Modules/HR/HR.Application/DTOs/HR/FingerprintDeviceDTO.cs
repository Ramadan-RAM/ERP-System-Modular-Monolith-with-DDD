// Application/HR/DTOs/FingerprintDeviceDTO.cs
namespace HR.Application.DTOs.HR
{
    public class FingerprintDeviceDTO
    {
        public int Id { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public string IPAddress { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int BranchId { get; set; }

        public string? BranchName { get; set; } // Optional for display
    }

}
