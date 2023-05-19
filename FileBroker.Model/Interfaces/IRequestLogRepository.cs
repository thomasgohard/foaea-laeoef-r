using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IRequestLogRepository
    {
        Task<int> AddAsync(RequestLogData requestLogData);
        Task<List<RequestLogData>> GetAllAsync();
        Task DeleteAllAsync();
    }
}
