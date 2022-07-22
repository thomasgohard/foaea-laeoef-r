using System.Collections.Generic;

namespace FileBroker.Model.Interfaces
{
    public interface IOutgoingFileManager
    {
        string CreateOutputFile(string fileBaseName, out List<string> errors);
    }
}
