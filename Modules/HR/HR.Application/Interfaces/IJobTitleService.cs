using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;


namespace HR.Application.HR.Interfaces
{
    public interface IJobTitleService
    {
        Task<Result<List<JobTitleDTO>>> GetAllAsync();
        Task<Result<JobTitleDTO?>> GetByIdAsync(int id);
        Task<Result<JobTitleDTO>> CreateAsync(JobTitleDTO dto);
        Task<Result<bool>> UpdateAsync(int id, JobTitleDTO dto);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
