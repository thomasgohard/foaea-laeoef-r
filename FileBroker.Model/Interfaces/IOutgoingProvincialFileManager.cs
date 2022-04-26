using System.Collections.Generic;

namespace FileBroker.Model.Interfaces
{
    public interface IOutgoingProvincialFileManager
    {
        string CreateOutputFile(string fileBaseName, out List<string> errors);
    }
}
