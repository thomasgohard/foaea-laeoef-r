using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers.Financials
{
    public class TransactionAPIBroker : ITransactionAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public TransactionAPIBroker(IAPIBrokerHelper apiHelper, string token)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<TransactionResult> InsertFaFrDe(string transactionType, SummFAFR_DE_Data newTransaction)
        {
            string apiCall = $"api/v1/SummFaFrDe?TransactionType={transactionType}";
            return await ApiHelper.PostData<TransactionResult, SummFAFR_DE_Data>(apiCall, newTransaction, token: Token);
        }
    }
}
