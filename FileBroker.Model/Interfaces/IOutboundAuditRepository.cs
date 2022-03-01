using System;
using System.Collections.Generic;

namespace FileBroker.Model.Interfaces
{
    public interface IOutboundAuditRepository
    {
        void InsertIntoOutboundAudit(string fileName, DateTime fileDate, bool fileCreated, string message);
    }
}
