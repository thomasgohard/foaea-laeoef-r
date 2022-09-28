using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILoginsAPIBroker
    {
        Task<List<Claim>> LoginAsync(LoginData2 loginData);

        Task<string> LogoutAsync(LoginData2 loginData);
    }
}
