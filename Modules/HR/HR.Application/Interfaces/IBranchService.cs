// Application/HR/Interfaces/IBranchService.cs
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;

namespace HR.Application.Interfaces.HR;
public interface IBranchService
{
    Task<Result<List<BranchDTO>>> GetAllAsync();
    Task<Result<BranchDTO?>> GetByIdAsync(int id);
    Task<Result<BranchDTO>> CreateAsync(BranchDTO dto);
    Task<Result<bool>> UpdateAsync(int id, BranchDTO dto);
    Task<Result<bool>> DeleteAsync(int id);
}
