using System.Text.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WASMAuthenticationDemo.Client.Exceptions;
using System.Text;

namespace WASMAuthenticationDemo.Client.Repository
{
    public class GenericRepository : IGenericRepository
    {
        private readonly HttpClient _httpClient;
        private string _token = "";

        public GenericRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _token = token;
            }
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            return await CallAPI<T>(HttpMethod.Get, uri, null);
        }

        public async Task<T> PostAsync<T>(string uri, T data)
        {
            return await CallAPI<T>(HttpMethod.Post, uri, data);
        }

        public async Task<TR> PostAsync<T, TR>(string uri, T data)
        {
            return await CallAPI<TR>(HttpMethod.Post, uri, data);
        }

        public async Task<T> PutAsync<T>(string uri, T data)
        {
            return await CallAPI<T>(HttpMethod.Put, uri, data);
        }

        private async Task<T> CallAPI<T>(HttpMethod method, string uri, object data) 
        {
            try
            {
                string jsonResult = string.Empty;
                var req = new HttpRequestMessage(method, uri);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (data != null)
                {
                    req.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                }
                if (!string.IsNullOrEmpty(_token))
                {
                    req.Headers.Add("Authorization", $"Bearer {_token}");
                }
                var response = await _httpClient.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    jsonResult = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<T>(jsonResult, options);
                    return json;
                }
                throw new HttpRequestExceptionEx(response.StatusCode, jsonResult);
            } catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType().Name} : {e.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(string uri)
        {
            await _httpClient.DeleteAsync(GetUri(uri));
        }

        private string GetUri(string uri) => $"{uri}";
    }
}
