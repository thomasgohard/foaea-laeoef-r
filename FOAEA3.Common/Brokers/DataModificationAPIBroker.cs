using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class DataModificationAPIBroker : IDataModificationAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public DataModificationAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> Update(DataModificationData dataModificationData)
        {
            string apiCall = $"api/v1/DataModifications/update";
            var messageData = await ApiHelper.PutData<StringData, DataModificationData>(apiCall, dataModificationData, token: Token);
            return messageData.Data;
        }

        public async Task<string> InsertCraSinPending(SinModificationData sinModificationData)
        {
            string apiCall = $"api/v1/DataModifications/SINpending/insert";
            var messageData = await ApiHelper.PostData<StringData, SinModificationData>(apiCall, sinModificationData, token: Token);
            return messageData.Data;
        }
        public async Task<string> DeleteCraSinPending(SinModificationData sinModificationData)
        {
            string apiCall = $"api/v1/DataModifications/SINpending/delete";
            var messageData = await ApiHelper.PostData<StringData, SinModificationData>(apiCall, sinModificationData, token: Token);
            return messageData.Data;
        }
    }
}
