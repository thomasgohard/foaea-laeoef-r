using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IFlatFileSpecificationRepository
    {
        Task<List<FlatFileSpecificationData>> GetFlatFileSpecificationsForFileAsync(int processId);
    }
}
