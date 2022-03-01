using FOAEA3.Data.Base;
using FOAEA3.Helpers;
using FOAEA3.Model;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class BaseAPI
    {
        private static string _APIroot;
        private static string _APIroot_Interception;
        private static string _APIroot_LicenceDenial;
        private static string _APIroot_Tracing;

        public static string APIroot
        {
            get => _APIroot;
            set => _APIroot = value + (value.EndsWith("/", StringComparison.Ordinal) ? "" : "/");
        }

        public static string APIroot_Interception
        {
            get => _APIroot_Interception;
            set => _APIroot_Interception = value + (value.EndsWith("/", StringComparison.Ordinal) ? "" : "/");
        }

        public static string APIroot_LicenceDenial
        {
            get => _APIroot_LicenceDenial;
            set => _APIroot_LicenceDenial = value + (value.EndsWith("/", StringComparison.Ordinal) ? "" : "/");
        }

        public static string APIroot_Tracing
        {
            get => _APIroot_Tracing;
            set => _APIroot_Tracing = value + (value.EndsWith("/", StringComparison.Ordinal) ? "" : "/");
        }

        private readonly TimeSpan DEFAULT_TIMEOUT = new(0, 20, 0);

        public BaseAPI()
        {

        }

        protected async Task<string> GetStringAsync(string api, string root = "")
        {

            string result = string.Empty;

            if (root == "")
                root = APIroot;

            using (var httpClient = new HttpClient())
            {

                httpClient.Timeout = DEFAULT_TIMEOUT;
                httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", SessionData.CurrentSubmitter);
                httpClient.DefaultRequestHeaders.Add("CurrentSubject", SessionData.FOAEAUser);

                var callResult = await httpClient.GetAsync(root + api);

                if (callResult.IsSuccessStatusCode)
                {
                    result = await callResult.Content.ReadAsStringAsync();
                }

            }

            return result;

        }

        protected async Task<T> GetDataAsync<T>(string api, string root = "")
            where T : class, new()
        {

            T result = null;

            if (root == "")
                root = APIroot;

            using (var httpClient = new HttpClient())
            {

                httpClient.Timeout = DEFAULT_TIMEOUT;
                httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", SessionData.CurrentSubmitter);
                httpClient.DefaultRequestHeaders.Add("CurrentSubject", SessionData.FOAEAUser);

                var callResult = await httpClient.GetAsync(root + api);

                if (callResult.IsSuccessStatusCode)
                {
                    string content = callResult.Content.ReadAsStringAsync().Result;
                    // content = content.Replace();
                    result = JsonConvert.DeserializeObject<T>(content);
                }
                else if (callResult.StatusCode == HttpStatusCode.InternalServerError)
                {
                    string content = callResult.Content.ReadAsStringAsync().Result;
                    var messages = JsonConvert.DeserializeObject<MessageDataList>(content);
                    ReferenceData.Instance().Messages.Merge(messages);
                }

            }

            if (result is null)
                result = new T();

            return result;

        }

        protected async Task<T> PostDataAsync<T, P>(string api, P data, string root = "")
            where T : class, new()
            where P : class
        {
            return await SendDataAsync<T, P>(api, data, "POST", root);
        }

        protected async Task<T> PutDataAsync<T, P>(string api, P data, string root = "")
            where T : class, new()
            where P : class
        {
            return await SendDataAsync<T, P>(api, data, "PUT", root);
        }

        private async Task<T> SendDataAsync<T, P>(string api, P data, string method, string root = "")
            where T : class, new()
            where P : class
        {

            T result = null;

            if (root == "")
                root = APIroot;

            using (var httpClient = new HttpClient())
            {

                httpClient.Timeout = DEFAULT_TIMEOUT;
                httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", SessionData.CurrentSubmitter);
                httpClient.DefaultRequestHeaders.Add("CurrentSubject", SessionData.FOAEAUser);

                string keyData = JsonConvert.SerializeObject(data);

                HttpResponseMessage callResult;
                using (var content = new StringContent(keyData, Encoding.UTF8, "application/json"))
                    callResult = method switch
                    {
                        "POST" => httpClient.PostAsync(root + api, content).Result,
                        "PUT" => httpClient.PutAsync(root + api, content).Result,
                        _ => null,
                    };

                if (callResult != null) // && (callResult.IsSuccessStatusCode))
                {
                    string content = await callResult.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(content);
                }

            }

            if (result is null)
                result = new T();

            return result;

        }

    }
}
