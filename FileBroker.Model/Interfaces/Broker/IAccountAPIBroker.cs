using FOAEA3.Model;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IAccountAPIBroker
    {
        Task<TokenData> CreateTokenAsync(FileBrokerLoginData loginData);
    }
}
