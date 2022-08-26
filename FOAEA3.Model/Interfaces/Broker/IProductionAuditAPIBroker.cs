﻿using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IProductionAuditAPIBroker
    {
        Task InsertAsync(string processName, string description, string audience, DateTime? completedDate = null);
        Task InsertAsync(ProductionAuditData productionAuditData);
    }
}