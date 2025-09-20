// File: Application/HR/Interfaces/IEmployeeLeaveService.cs
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;


namespace HR.Application.HR.Interfaces
{
    public interface IEmployeeLeaveService
    {
        Task<Result<List<EmployeeLeaveDTO>>> GetAllAsync();
        Task<Result<EmployeeLeaveDTO?>> GetByIdAsync(int id);
        Task<Result<EmployeeLeaveDTO>> CreateAsync(EmployeeLeaveDTO dto);
        Task<Result<bool>> UpdateAsync(int id, EmployeeLeaveDTO dto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
