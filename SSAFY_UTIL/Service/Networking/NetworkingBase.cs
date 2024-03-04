using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SSAFY_UTIL.Service.Networking
{
    public class NetworkingBase
    {
        protected readonly HttpClient client;

        protected NetworkingBase(Uri uri, List<KeyValuePair<string, string>>? headers)
        {
            client = new()
            {
                BaseAddress = uri,
            };

            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                }
            }
        }

        protected async Task<(HttpStatusCode, string)> GetAsString(
            string path, 
            List<KeyValuePair<string, string>>? parameters,
            List<KeyValuePair<string, string>>? headers)
        {
            if (parameters != null)
            {
                string parameter = "?";
                parameter += BuildUrlParams(parameters);
                path += parameter;
            }

            HttpResponseMessage response;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, path))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                response = await client.SendAsync(requestMessage);
            }

            string result = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, result);
        }

        protected async Task<(HttpStatusCode, string)> PostAsString(
            string path,
            List<KeyValuePair<string, string>>? parameters,
            string? body,
            List<KeyValuePair<string, string>>? headers)
        {
            if (parameters != null)
            {
                string parameter = "?";
                parameter += BuildUrlParams(parameters);
                path += parameter;
            }

            HttpResponseMessage response;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, path))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                if (body != null)
                {
                    requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }
                response = await client.SendAsync(requestMessage);
            }

            string result = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, result);
        }

        private string BuildUrlParams(List<KeyValuePair<string, string>> param)
        {
            string result = "";
            foreach (var item in param.Select((value, index) => (value, index)))
            {
                var pair = item.value;
                int index = item.index;

                if (index != 0)
                    result += "&";
                result += HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value);
            }

            return result;
        }
    }
}
