using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IProcessParameterRepository
    {
        Task<string> GetValueForParameterAsync(int processId, string parameter);
        Task<ProcessCodeData> GetProcessCodesAsync(int processId);
    }
}
