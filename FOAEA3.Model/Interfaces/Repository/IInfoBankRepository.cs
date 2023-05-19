using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IInfoBankRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<InfoBankData>> GetInfoBanksAsync();
    }
}
