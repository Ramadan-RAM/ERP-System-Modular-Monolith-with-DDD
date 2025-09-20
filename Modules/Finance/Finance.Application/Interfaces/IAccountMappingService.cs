using System.Threading.Tasks;

namespace Finance.Application.Interfaces
{
    public interface IAccountMappingService
    {
        Task<int> GetSalaryExpenseAccountId(int departmentId);
        Task<int> GetCashAccountId(int departmentId, string employeeCode);
    }
}
