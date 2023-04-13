using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IOutgoingFileManager
    {
        Task<(string, List<string>)> CreateOutputFileAsync(string fileBaseName);
    }
}
