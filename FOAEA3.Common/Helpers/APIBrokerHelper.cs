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
        private static readonly TimeSpan DEFAULT_TIMEOUT = new(0, 0, 30);

        private string _APIroot;
        public string CurrentSubmitter { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentLanguage { get; set; }
        public Func<Task<string>> GetRefreshedToken { get; set; }


        public MessageDataList Messages { get; set; }

        public string APIroot
        {
            get => _APIroot;
            set => _APIroot = value + (value.EndsWith("/", StringComparison.Ordinal) ? "" : "/");
        }

        public APIBrokerHelper(string apiRoot = "", string currentSubmitter = "", string currentUser = "",
                               Func<Task<string>> getRefreshedToken = null)
        {
            APIroot = apiRoot;
            CurrentSubmitter = currentSubmitter;
            CurrentUser = currentUser;
            GetRefreshedToken = getRefreshedToken;
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

        public async Task<string> GetStringAsync(string api, string root = "",
                                                 int maxAttempts = GlobalConfiguration.MAX_API_ATTEMPTS,
                                                 string token = null)
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
                    if (token is not null && !string.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    if (!string.IsNullOrEmpty(CurrentLanguage))
                        httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

                    var callResult = await httpClient.GetAsync(root + api);

                    if (callResult.IsSuccessStatusCode)
                    {
                        result = await callResult.Content.ReadAsStringAsync();
                        completed = true;
                    }
                    else if (callResult.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (GetRefreshedToken is null)
                            completed = true;
                        else
                        {
                            token = await GetRefreshedToken();
                            if (string.IsNullOrEmpty(token))
                                completed = true; // API failed and refreshed token expired
                            else
                                attemptCount++;
                        }
                    }
                    else
                        completed = true;
                }
                catch (Exception e)
                {
                    attemptCount++;
                    Thread.Sleep(GlobalConfiguration.TIME_BETWEEN_RETRIES); // wait half a second between each attempt
                    if (attemptCount == maxAttempts)
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

        public async Task<T> GetDataAsync<T>(string api, string root = "", string token = null)
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
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
                        httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
                        if (token is not null && !string.IsNullOrEmpty(token))
                            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
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
                        else if (callResult.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            if (GetRefreshedToken is null)
                                completed = true;
                            else
                            {
                                token = await GetRefreshedToken();
                                if (string.IsNullOrEmpty(token))
                                    completed = true; // API failed and refreshed token expired
                                else
                                    attemptCount++;
                            }
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

        public async Task<T> PostDataAsync<T, P>(string api, P data, string root = "", string token = null)
            where T : class, new()
            where P : class
        {
            return await SendDataAsync<T, P>(api, data, "POST", root, token);
        }

        public async Task<T> PutDataAsync<T, P>(string api, P data, string root = "", string token = null)
            where T : class, new()
            where P : class
        {
            return await SendDataAsync<T, P>(api, data, "PUT", root, token);
        }

        public async Task<string> PostDataGetStringAsync<P>(string api, P data, string root = "", string token = null) where P : class
        {
            return await SendDataGetStringAsync<P>(api, data, "POST", root, token);
        }

        public async Task<string> PutDataGetStringAsync<P>(string api, P data, string root = "", string token = null) where P : class
        {
            return await SendDataGetStringAsync<P>(api, data, "PUT", root, token);
        }

        public async Task SendCommandAsync(string api, string root = "", string token = null)
        {
            await SendCommandAsync(api, "PUT", root, token);
        }

        private async Task<T> SendDataAsync<T, P>(string api, P data, string method, string root = "",
                                                  string token = null)
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
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
                    httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
                    if (token is not null && !string.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
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

                    if (callResult != null)
                    {
                        if (callResult.IsSuccessStatusCode)
                        {
                            string content = await callResult.Content.ReadAsStringAsync();
                            result = JsonConvert.DeserializeObject<T>(content);
                            completed = true;
                        }
                        else if (callResult.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            if (GetRefreshedToken is null)
                                completed = true;
                            else
                            {
                                token = await GetRefreshedToken();
                                if (string.IsNullOrEmpty(token))
                                    completed = true; // API failed and refreshed token expired
                                else
                                    attemptCount++;
                            }
                        }
                    }
                    else 
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

        private async Task<string> SendDataGetStringAsync<P>(string api, P data, string method,
                                                             string root = "", string token = null)
                                            where P : class
        {
            string result = string.Empty;

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
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
                    httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
                    if (token is not null && !string.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
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

                    if (callResult != null)
                    {
                        if (callResult.IsSuccessStatusCode)
                        {
                            result = await callResult.Content.ReadAsStringAsync();
                            completed = true;
                        }
                        else if (callResult.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            if (GetRefreshedToken is null)
                                completed = true;
                            else
                            {
                                token = await GetRefreshedToken();
                                if (string.IsNullOrEmpty(token))
                                    completed = true;
                                else
                                    attemptCount++;
                            }
                        }
                    }
                    else
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

        private async Task SendCommandAsync(string api, string method, string root = "", string token = null)
        {
            if (root == "")
                root = APIroot;

            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            if (token is not null && !string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
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

        public async Task<HttpResponseMessage> PostJsonFileAsync(string api, string jsonData,
                                                                 string rootAPI = null, string token = null)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            if (token is not null && !string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            if (!string.IsNullOrEmpty(CurrentLanguage))
                httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            string root = rootAPI ?? APIroot;
            return await httpClient.PostAsync(root + api, content);
        }

        public async Task<HttpResponseMessage> PostFlatFileAsync(string api, string flatFileData,
                                                                 string rootAPI = null, string token = null)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = DEFAULT_TIMEOUT;
            httpClient.DefaultRequestHeaders.Add("CurrentSubmitter", CurrentSubmitter);
            httpClient.DefaultRequestHeaders.Add("CurrentSubject", CurrentUser);
            if (token is not null && !string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            if (!string.IsNullOrEmpty(CurrentLanguage))
                httpClient.DefaultRequestHeaders.Add("Accept-Language", CurrentLanguage);

            using var content = new StringContent(flatFileData, Encoding.UTF8, "text/plain");

            string root = rootAPI ?? APIroot;
            return await httpClient.PostAsync(root + api, content);
        }

    }

}
