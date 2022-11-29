using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class ProductionAuditAPIBroker : IProductionAuditAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ProductionAuditAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task InsertAsync(string processName, string description, string audience, DateTime? completedDate = null)
        {
            var productionAuditData = new ProductionAuditData
            {
                Process_name = processName,
                Description = description,
                Audience = audience,
                Compl_dte = completedDate
            };

            _ = await ApiHelper.PostDataAsync<ProductionAuditData, ProductionAuditData>("api/v1/productionAudits",
                                                                             productionAuditData, token: Token);
        }

        public async Task InsertAsync(ProductionAuditData productionAuditData)
        {
            _ = await ApiHelper.PostDataAsync<ProductionAuditData, ProductionAuditData>("api/v1/productionAudits",
                                                                              productionAuditData, token: Token);
        }
    }
}
