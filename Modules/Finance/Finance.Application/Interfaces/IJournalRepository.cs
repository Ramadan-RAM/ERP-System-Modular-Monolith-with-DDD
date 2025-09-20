using System.Threading.Tasks;
using Finance.Domain.Entities;

namespace Finance.Application.Interfaces
{
    public interface IJournalRepository
    {
        Task AddAsync(JournalEntry entry);
    }
}
