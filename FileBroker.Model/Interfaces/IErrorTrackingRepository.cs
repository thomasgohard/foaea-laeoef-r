using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IErrorTrackingRepository
    {
        Task MessageBrokerErrorAsync(string errorType, string errorSubject, Exception e, bool displayExceptionError, DataRow row = null);

        Task MessageBrokerErrorAsync(string errorSubject, string errorMessage);
    }
}
