using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IOutgoingFileManager
    {
        Task<string> CreateOutputFileAsync(string fileBaseName, List<string> errors);
    }
}
