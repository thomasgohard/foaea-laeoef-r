using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FOAEA3.Common.Helpers
{
    public class APIBrokerHelper : IAPIBrokerHelper
    {
        private static readonly TimeSpan DEFAULT_TIMEOUT = new(0, 20, 0);

        private string _APIroot;
        public string CurrentSubmitter { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentLanguage { get; set; }

        public MessageDataList Messages { get; set; }

        public string APIroot
        {
            get => _APIroot;
            set => _APIroot = value + (value.EndsWith("/", StringComparison.Ordinal) ? "" : "/");
        }

        public APIBrokerHelper(string apiRoot = "", string currentSubmitter = "", string currentUser = "")
        {
            APIroot = apiRoot;
            CurrentSubmitter = currentSubmitter;
            CurrentUser = currentUser;
        }

        public static async Task<T> GetDataFromRequestBodyAsync<T>(HttpRequest request)
        {
            string bodyDataAsJSON;
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                bodyDataAsJSON = await reader.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<T>(bodyDataAsJSON);
        }

        public async Task<string> GetStringAsync(string api, string root = "", int maxAttempts = GlobalConfiguration.MAX_API_ATTEMPTS)
        {

            string result = string.Empty;

            if (root == "")
                root = APIroot;

            int attemptCount = 0;
            bool completed = false;

            while ((attemptCount < maxAttempts) && (!completed))
            {
                try
                {
                    using var httpClient = new HttpClient();

                    httpClient.Timeout = DEFAULT_TIMEOUT;
                    httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
                    httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
                    if (!string.IsNullOrEmpty(CurrentLanguage))
                        httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

                    var callResult = await httpClient.GetAsync(root + api);

                    if (callResult.IsSuccessStatusCode)
                    {
                        result = await callResult.Content.ReadAsStringAsync();
                    }
                    completed = true;
                }
                catch (Exception e)
                {
                    attemptCount++;
                    Thread.Sleep(GlobalConfiguration.TIME_BETWEEN_RETRIES); // wait half a second between each attempt
                    if (attemptCount == GlobalConfiguration.MAX_API_ATTEMPTS)
                    {
                        // log error
                        Messages = new MessageDataList();
                        var errorMessageData = new MessageData(EventCode.UNDEFINED, "Error", e.Message, MessageType.Error);
                        Messages.Add(errorMessageData);
                    }
                }
            }

            return result;

        }

        public async Task<T> GetDataAsync<T>(string api, string root = "")
            where T : class, new()
        {

            T result = null;

            if (root == "")
                root = APIroot;

            using (var httpClient = new HttpClient())
            {
                int attemptCount = 0;
                bool completed = false;

                while ((attemptCount < GlobalConfiguration.MAX_API_ATTEMPTS) && (!completed))
                {
                    try
                    {

                        httpClient.Timeout = DEFAULT_TIMEOUT;
                        httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
                        httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
                        if (!string.IsNullOrEmpty(CurrentLanguage))
                            httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

                        var callResult = await httpClient.GetAsync(root + api);

                        if (callResult.IsSuccessStatusCode)
                        {
                            string content = await callResult.Content.ReadAsStringAsync();
                            result = JsonConvert.DeserializeObject<T>(content);
                        }
                        else if (callResult.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            string content = await callResult.Content.ReadAsStringAsync();
                            Messages = JsonConvert.DeserializeObject<MessageDataList>(content);
                        }
                        completed = true;
                    }
                    catch (Exception e)
                    {
                        attemptCount++;
                        Thread.Sleep(GlobalConfiguration.TIME_BETWEEN_RETRIES); // wait half a second between each attempt
                        if (attemptCount == GlobalConfiguration.MAX_API_ATTEMPTS)
                        {
                            // log error
                            Messages = new MessageDataList();
                            var errorMessageData = new MessageData(EventCode.UNDEFINED, "Error", e.Message, MessageType.Error);
                            Messages.Add(errorMessageData);
                        }
                    }
                }

            }

            if (result is null)
                result = new T();

            return result;

        }

        public async Task<T> PostDataAsync<T, P>(string api, P data, string root = "")
            where T : class, new()
            where P : class
        {
            return await SendDataAsync<T, P>(api, data, "POST", root);
        }

        public async Task<T> PutDataAsync<T, P>(string api, P data, string root = "")
            where T : class, new()
            where P : class
        {
            return await SendDataAsync<T, P>(api, data, "PUT", root);
        }

        public async Task SendCommandAsync(string api, string root = "")
        {
            await SendCommandAsync(api, "PUT", root);
        }

        private async Task<T> SendDataAsync<T, P>(string api, P data, string method, string root = "")
            where T : class, new()
            where P : class
        {

            T result = null;

            if (root == "")
                root = APIroot;

            int attemptCount = 0;
            bool completed = false;

            while ((attemptCount < GlobalConfiguration.MAX_API_ATTEMPTS) && (!completed))
            {
                try
                {
                    using var httpClient = new HttpClient();

                    httpClient.Timeout = DEFAULT_TIMEOUT;
                    httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
                    httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
                    if (!string.IsNullOrEmpty(CurrentLanguage))
                        httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

                    string keyData = JsonConvert.SerializeObject(data);

                    HttpResponseMessage callResult;
                    using (var content = new StringContent(keyData, Encoding.UTF8, "application/json"))
                        callResult = method switch
                        {
                            "POST" => await httpClient.PostAsync(root + api, content),
                            "PUT" => await httpClient.PutAsync(root + api, content),
                            _ => null,
                        };

                    if (callResult != null) // && (callResult.IsSuccessStatusCode))
                    {
                        string content = await callResult.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<T>(content);
                    }

                    completed = true;
                }
                catch (Exception e)
                {
                    attemptCount++;
                    Thread.Sleep(GlobalConfiguration.TIME_BETWEEN_RETRIES); // wait half a second between each attempt
                    if (attemptCount == GlobalConfiguration.MAX_API_ATTEMPTS)
                    {
                        // log error
                        Messages = new MessageDataList();
                        var errorMessageData = new MessageData(EventCode.UNDEFINED, "Error", e.Message, MessageType.Error);
                        Messages.Add(errorMessageData);
                    }
                }
            }

            if (result is null)
                result = new T();

            return result;

        }

        private async Task SendCommandAsync(string api, string method, string root = "")
        {
            if (root == "")
                root = APIroot;

            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            if (!string.IsNullOrEmpty(CurrentLanguage))
                httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

            HttpResponseMessage callResult;
            callResult = method switch
            {
                "POST" => await httpClient.PostAsync(root + api, null),
                "PUT" => await httpClient.PutAsync(root + api, null),
                _ => null,
            };

        }

        public async Task<HttpResponseMessage> PostJsonFileAsync(string api, string jsonData, string rootAPI = null)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            if (!string.IsNullOrEmpty(CurrentLanguage))
                httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            string root = rootAPI ?? APIroot;
            return await httpClient.PostAsync(root + api, content);
        }

        public async Task<HttpResponseMessage> PostJsonFileWithTokenAsync(string api, string jsonData, string token, string rootAPI = null)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            httpClient.DefaultRequestHeaders.Add("Authentication", "Bearer " + token);
            if (!string.IsNullOrEmpty(CurrentLanguage))
                httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            string root = rootAPI ?? APIroot;
            return await httpClient.PostAsync(root + api, content);
        }

        public async Task<HttpResponseMessage> PostFlatFileAsync(string api, string flatFileData, string rootAPI = null)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            if (!string.IsNullOrEmpty(CurrentLanguage))
                httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

            using var content = new StringContent(flatFileData, Encoding.UTF8, "text/plain");

            string root = rootAPI ?? APIroot;
            return await httpClient.PostAsync(root + api, content);
        }

    }

}
