using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationLifeStatesAPIBroker
    {
        Task<DataList<ApplicationLifeStateData>> GetApplicationLifeStatesAsync();
    }
}
