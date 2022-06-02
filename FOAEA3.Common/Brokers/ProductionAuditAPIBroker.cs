using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System;

namespace FOAEA3.Common.Brokers
{
    public class ProductionAuditAPIBroker : IProductionAuditAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ProductionAuditAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public void Insert(string processName, string description, string audience, DateTime? completedDate = null)
        {
            var productionAuditData = new ProductionAuditData
            {
                Process_name = processName,
                Description = description,
                Audience = audience,
                Compl_dte = completedDate
            };

            _ = ApiHelper.PostDataAsync<ProductionAuditData, ProductionAuditData>("api/v1/productionAudits",
                                                                                  productionAuditData).Result;
        }

        public void Insert(ProductionAuditData productionAuditData)
        {
            _ = ApiHelper.PostDataAsync<ProductionAuditData, ProductionAuditData>("api/v1/productionAudits",
                                                                                  productionAuditData).Result;
        }
    }
}
