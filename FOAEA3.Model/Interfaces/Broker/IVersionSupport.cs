using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IVersionSupport
    {
        Task<string> GetVersionAsync(string token);
        Task<string> GetConnectionAsync(string token);
    }

}
