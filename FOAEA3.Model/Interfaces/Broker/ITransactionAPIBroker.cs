using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ITransactionAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<TransactionResult> InsertFaFrDe(string transactionType, SummFAFR_DE_Data newTransaction);
    }
}
