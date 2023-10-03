using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IDataModificationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<string> Update(DataModificationData dataModificationData);
        Task<string> InsertCraSinPending(SinModificationData sinModificationData);
        Task<string> DeleteCraSinPending(SinModificationData sinModificationData);
    }
}
