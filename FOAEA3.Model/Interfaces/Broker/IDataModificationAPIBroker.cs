using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IDataModificationAPIBroker
    {
        Task<string> Update(DataModificationData dataModificationData);
        Task<string> InsertCraSinPending(SinModificationData sinModificationData);
        Task<string> DeleteCraSinPending(SinModificationData sinModificationData);
    }
}
