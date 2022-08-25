using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IVersionSupport
    {
        string GetVersion();
        string GetConnection();
    }

    public interface IVersionAsyncSupport
    {
        Task<string> GetVersionAsync();
        Task<string> GetConnectionAsync();
    }
}
