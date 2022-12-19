using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IFundsAvailableRepository
    {
        Task<List<FundsAvailableIncomingTrainingData>> GetFundsAvailableIncomingTrainingData(string batchId);
    }
}
