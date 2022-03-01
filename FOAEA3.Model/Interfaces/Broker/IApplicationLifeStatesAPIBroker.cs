using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationLifeStatesAPIBroker
    {
        DataList<ApplicationLifeStateData> GetApplicationLifeStates();
    }
}
