using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;

namespace HR.Application.HR.Interfaces
{
    public interface IFingerprintDeviceService
    {
        Task<Result<List<FingerprintDeviceDTO>>> GetAllAsync();
        Task<Result<FingerprintDeviceDTO?>> GetByIdAsync(int id);
        Task<Result<FingerprintDeviceDTO>> CreateAsync(FingerprintDeviceDTO dto);
        Task<Result<bool>> UpdateAsync(int id, FingerprintDeviceDTO dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<List<FingerprintDeviceDTO>>> GetByBranchIdAsync(int branchId);
    }
}
