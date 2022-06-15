using System.Collections.Generic;

namespace FileBroker.Model.Interfaces
{
    public interface IRequestLogRepository
    {
        int Add(RequestLogData requestLogData);
        List<RequestLogData> GetAll();
        void DeleteAll();
    }
}
