using FOAEA3.Model.Constants;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IAPIBrokerHelper
    {
        string APIroot { get; set; }
        string CurrentSubmitter { get; set; }
        string CurrentUser { get; set; }
        string CurrentLanguage { get; set; }
        Func<string> GetToken { get; set; }
        Func<Task<string>> GetRefreshedToken { get; set; }

        MessageDataList ErrorData { get; set; }
        Task<T> GetData<T>(string api, string root = "", string token = null) where T : new();
        Task<T> GetData<T, P>(string api, P data, string root = "", string token = null) where T : new();
        Task<string> GetString(string api, string root = "", int maxAttempts = GlobalConfiguration.MAX_API_ATTEMPTS, string token = null);
        Task<HttpResponseMessage> PostJsonFile(string api, string jsonData, string rootAPI = null, string token = null);
        Task<HttpResponseMessage> PostFlatFile(string api, string flatFileData, string rootAPI = null, string token = null);
        Task<T> PostData<T, P>(string api, P data, string root = "", string token = null) where T : class, new() where P : class;
        Task<T> PutData<T, P>(string api, P data, string root = "", string token = null) where T : class, new() where P : class;
        Task<string> PostDataGetString<P>(string api, P data, string root = "", string token = null) where P : class;
        Task<string> PutDataGetString<P>(string api, P data, string root = "", string token = null) where P : class;
        Task SendCommand(string api, string root = "", string token = null);
    }
}
