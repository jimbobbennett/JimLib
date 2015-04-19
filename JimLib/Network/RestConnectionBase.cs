using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JimBobBennett.JimLib.Extensions;
using JimBobBennett.JimLib.Xml;
using ModernHttpClient;
using Newtonsoft.Json;

namespace JimBobBennett.JimLib.Network
{
    /// <summary>
    /// Provides the base class for connections - this file can be shared between the mono and win versions
    /// as the using directives match despite the assemblies being different.
    /// </summary>
    public class RestConnectionBase : IRestConnection
    {
        public async Task<byte[]> MakeRawGetRequestAsync(string baseUrl, string resource = "/",
            string username = null, string password = null, int timeout = 10000,
            Dictionary<string, string> headers = null)
        {
            using (var clientHandler = new NativeMessageHandler {UseCookies = false})
            {
                if (!string.IsNullOrEmpty(username))
                    clientHandler.Credentials = new NetworkCredential(username, password);

                using (var client = new HttpClient(clientHandler))
                {
                    try
                    {
                        client.BaseAddress = new Uri(baseUrl);
                        client.Timeout = TimeSpan.FromMilliseconds(timeout);

                        if (headers != null)
                        {
                            foreach (var h in headers)
                                client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        }

                        var requestUri = new Uri(resource, UriKind.Relative);
                        
                        var getResponse = await client.GetAsync(requestUri);
                        
                        if (getResponse.IsSuccessStatusCode)
                            return await getResponse.Content.ReadAsByteArrayAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Requesting " + baseUrl + " and resource " + resource + " - Failed to make network request: " + ex.Message);
                    }
                }
            }

            return null;
        }

        public async Task<RestResponse<T>> MakeRequestAsync<T, TData>(Method method, ResponseType responseType, 
            string baseUrl, string resource = "/", string username = null, string password = null,
            int timeout = 10000, Dictionary<string, string> headers = null, TData postData = null)
            where T : class, new()
            where TData : class
        {
            using (var clientHandler = new HttpClientHandler{UseCookies = false})
            {
                if (!string.IsNullOrEmpty(username))
                    clientHandler.Credentials = new NetworkCredential(username, password);

                using (var client = CreateClient(clientHandler, baseUrl, timeout, headers))
                {
                    try
                    {
                        var requestUri = new Uri(resource, UriKind.Relative);
                        
                        switch (method)
                        {
                            case Method.Get:
                                return await ProcessResponse<T>(responseType, await client.GetAsync(requestUri));

                            case Method.Post:
                                var postResponse = await client.PostAsync(requestUri, SerializePostData(postData, responseType));
                                return await ProcessResponse<T>(responseType, postResponse);
                        }

                        throw new ArgumentException("method");
                    }
                    catch (Exception ex)
                    {
                        return new RestResponse<T>(ex.Message, -1,  null);
                    }
                }
            }
        }

        private static async Task<RestResponse<T>> ProcessResponse<T>(ResponseType responseType, 
            HttpResponseMessage response)
            where T : class, new()
        {
            T responseObject = null;

            var statusCode = (int) response.StatusCode;
            var message = response.ReasonPhrase;

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!responseBody.IsNullOrEmpty())
                    responseObject = DeserializeResponse<T>(responseBody, responseType);
            }

            return new RestResponse<T>(message, statusCode, responseObject);
        }

        private static HttpClient CreateClient(HttpMessageHandler clientHandler, string baseUrl, int timeout, 
            Dictionary<string, string> headers)
        {
            var client = new HttpClient(clientHandler)
            {
                BaseAddress = new Uri(baseUrl), 
                Timeout = TimeSpan.FromMilliseconds(timeout)
            };

            if (headers != null)
            {
                foreach (var h in headers)
                    client.DefaultRequestHeaders.Add(h.Key, h.Value);
            }

            return client;
        }

        private static HttpContent SerializePostData<T>(T postData, ResponseType responseType)
            where T : class
        {
            if (postData == null)
                return new StringContent(string.Empty);
            
            if (responseType == ResponseType.Xml)
                throw new NotSupportedException();

            var jsonRequest = JsonConvert.SerializeObject(postData);
            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }

        private static T DeserializeResponse<T>(string responseBody, ResponseType responseType)
            where T : class, new()
        {
            if (string.IsNullOrEmpty(responseBody))
                return null;

            if (responseType == ResponseType.Xml)
                return new XmlDeserializer().Deserialize<T>(responseBody);

            return JsonConvert.DeserializeObject<T>(responseBody);
        }
    }
}