using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SafeMessenge.Services
{
    public class ApiService
    {
        private readonly string apiUrl = "https://63499aef5df952851403e3b2.mockapi.io/api/";
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(apiUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public T Read<T>(string request)
         where T : IResponseModel
        {
            return Task.Run(() => ExecuteGetRequest<T>(request)).Result;
        }

        public IEnumerable<T> ReadAll<T>(string request)
         where T : IResponseModel
        {
            return Task.Run(() => ExecuteGetAllRequest<T>(request)).Result;
        }

        private IEnumerable<T> ExecuteGetAllRequest<T>(string endpoint) where T : IResponseModel
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Content = GetEmptyRequestContent();
            var responce = _client.Send(request);
            //responce.EnsureSuccessStatusCode();
            var stream = responce.Content.ReadAsStream();
            IEnumerable<T> res = Array.Empty<T>();

            using (StreamReader sr = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new();
                res = serializer.Deserialize<IEnumerable<T>>(reader);
            }
            return res ?? Array.Empty<T>();
        }

        private async Task<T> ExecuteGetRequest<T>(string endpoint) where T : IResponseModel
        {
            
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Content = GetEmptyRequestContent();
                var responce = _client.Send(request);
                //responce.EnsureSuccessStatusCode();
                return ReadJsonAsync<T>(await responce.Content.ReadAsStreamAsync());
        }

        private StringContent GetEmptyRequestContent()
        {
            return new StringContent(string.Empty, Encoding.UTF8);
        }

        private static T ReadJsonAsync<T>(Stream s)
        {
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public async Task<T> SendPostRequest<T>(string endpoint,object create_object) where T : IResponseModel
        {
            var postData = new StringContent(JsonConvert.SerializeObject(create_object));
            postData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync(endpoint, postData);
            return ReadJsonAsync<T>(await response.Content.ReadAsStreamAsync());
        }
    }
}
