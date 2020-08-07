using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace issue_indexer_client.Models {
    public class RequestSender {

        private readonly Uri apiBaseURL;

        public RequestSender(string uri) {
            apiBaseURL = new Uri(uri);
        }

        public async Task<bool> Login(LoginModel loginModel) {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, apiBaseURL + "users/login") {
                Content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json")
            };
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Register(RegisterModel registerModel) {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, apiBaseURL + "users/register") {
                Content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json")
            };
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<T>> Post<T>(string path, object content) {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, apiBaseURL + path) {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            if (response.IsSuccessStatusCode) {
                return DeserializeJsonFromStream<List<T>>(stream);
            }
            var err_content = await StreamToStringAsync(stream);
            throw new ApiException {
                StatusCode = (int)response.StatusCode,
                Content = err_content
            };

        }

        private static T DeserializeJsonFromStream<T>(Stream stream) {
            if (stream == null || stream.CanRead == false)
                return default;

            using var sr = new StreamReader(stream);
            using var jtr = new JsonTextReader(sr);
            var js = new JsonSerializer();
            var searchResult = js.Deserialize<T>(jtr);
            return searchResult;
        }

        private static async Task<string> StreamToStringAsync(Stream stream) {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }

        private class ApiException : Exception {
            public int StatusCode { get; set; }
            public string Content { get; set; }
        }
    }

    
}
