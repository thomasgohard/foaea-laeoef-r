using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IVersionSupport
    {
        Task<string> GetVersionAsync();
        Task<string> GetConnectionAsync();
    }

}
