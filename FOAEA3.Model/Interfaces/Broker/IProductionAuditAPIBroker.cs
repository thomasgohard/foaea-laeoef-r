using System;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IProductionAuditAPIBroker
    {
        void Insert(string processName, string description, string audience, DateTime? completedDate = null);
        void Insert(ProductionAuditData productionAuditData);
    }
}
