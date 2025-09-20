using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;


namespace HR.Application.HR.Interfaces
{

    public interface IPayslipService
    {
        Task<Result<List<PayslipDTO>>> GetAllAsync();
        Task<Result<PayslipDTO?>> GetByIdAsync(int id);
        Task<Result<PayslipDTO>> CreateAsync(PayslipDTO dto);
        Task<Result<bool>> UpdateAsync(int id, PayslipDTO dto);
        Task<Result<bool>> DeleteAsync(int id);

        Task<Result<PayslipDTO>> GeneratePayslipAsync(int employeeId, int year, int month);
        Task<Result<List<PayslipDTO>>> GeneratePayslipsForAllAsync(int year, int month);
    }


}
