using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IControlBatchRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<ControlBatchData>> GetFADAReadyBatchAsync(string EnfSrv_Source_Cd = "", string DAFABatchID = "");
        Task<(string, string, string, string)> CreateXFControlBatchAsync(ControlBatchData values);

    }
}
