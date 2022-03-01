using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FileBroker.Model.Interfaces
{
    public interface IErrorTrackingRepository
    {
        void MessageBrokerError(string errorType, string errorSubject, Exception e, bool displayExceptionError, DataRow row = null);
    }
}
