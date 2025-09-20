using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;

namespace HR.Application.Interfaces.HR
{
    public interface IDepartmentService
    {
        Task<Result<List<DepartmentDTO>>> GetAllAsync();
        Task<Result<DepartmentDTO?>> GetByIdAsync(int id);
        Task<Result<DepartmentDTO>> CreateAsync(DepartmentDTO dto);
        Task<Result<bool>> UpdateAsync(int id, DepartmentDTO dto);
        Task<Result<bool>> DeleteAsync(int id);
    }



}
