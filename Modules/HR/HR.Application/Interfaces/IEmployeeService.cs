using ERPSys.SharedKernel;
using ERPSys.SharedKernel.Common;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;

namespace HR.Application.HR.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<PagedList<EmployeeDTO>>> GetAllAsync(int pageIndex, int pageSize);
        Task<Result<EmployeeDTO>> GetByIdAsync(int id);
        Task<Result<EmployeeDTO>> CreateAsync(EmployeeDTO employeeDto);
        Task<Result<bool>> UpdateAsync(int id, EmployeeDTO employeeDto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<bool>> SoftDeleteAsync(int id);
        Task<Result<List<string>>> GetDepartmentsAsync();
        Task<Result<List<string>>> GetJobTitlesAsync();
        Task<Result<List<string>>> GetJobTitlesByDepartmentNameAsync(string department);
        Task<Result<PagedList<EmployeeDTO>>> SearchAsync(string query, int pageIndex, int pageSize);
        Task<Result<List<string>>> SearchSuggestAppendAsync(string query);
    }
}
