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
        Func<Task<string>> GetRefreshedToken { get; set; }

        MessageDataList Messages { get; set; }
        Task<T> GetDataAsync<T>(string api, string root = "", string token = null) where T : new();
        Task<T> GetDataAsync<T, P>(string api, P data, string root = "", string token = null) where T : new();
        Task<string> GetStringAsync(string api, string root = "", int maxAttempts = GlobalConfiguration.MAX_API_ATTEMPTS, string token = null);
        Task<HttpResponseMessage> PostJsonFileAsync(string api, string jsonData, string rootAPI = null, string token = null);
        Task<HttpResponseMessage> PostFlatFileAsync(string api, string flatFileData, string rootAPI = null, string token = null);
        Task<T> PostDataAsync<T, P>(string api, P data, string root = "", string token = null) where T : class, new() where P : class;
        Task<T> PutDataAsync<T, P>(string api, P data, string root = "", string token = null) where T : class, new() where P : class;
        Task<string> PostDataGetStringAsync<P>(string api, P data, string root = "", string token = null) where P : class;
        Task<string> PutDataGetStringAsync<P>(string api, P data, string root = "", string token = null) where P : class;
        Task SendCommandAsync(string api, string root = "", string token = null);
    }
}
