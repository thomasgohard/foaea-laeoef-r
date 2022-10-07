using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IAccountAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<TokenData> CreateTokenAsync(FileBrokerLoginData loginData);
    }
}
