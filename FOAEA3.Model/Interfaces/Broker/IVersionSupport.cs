using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IVersionSupport
    {
        Task<string> GetVersion();
        Task<string> GetConnection();
    }

}
