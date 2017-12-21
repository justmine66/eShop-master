using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Resilience.Http
{
    public class StandardHttpClient : IHttpClient
    {
        readonly HttpClient _httpClient;
        readonly ILogger<StandardHttpClient> _logger;
        readonly IHttpContextAccessor _httpContextAccessor;

        public StandardHttpClient(ILogger<StandardHttpClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._httpClient = new HttpClient();
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<HttpResponseMessage> DeleteAsync(
            string uri,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);

            this.SetAuthorizationHeader(request);

            if (!string.IsNullOrEmpty(authorizationMethod))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                request.Headers.Add("x-requestid", requestId);
            }

            return await this._httpClient.SendAsync(request);
        }

        public async Task<string> GetStringAsync(
            string uri,
            string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            if (!string.IsNullOrEmpty(authorizationToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            var response = await this._httpClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostAsync<T>(
            string uri,
            T item,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return await this.DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(
            string uri,
            T item,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return await this.DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        private void SetAuthorizationHeader(HttpRequestMessage httpRequestMessage)
        {
            var authorizationHeader = this._httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                httpRequestMessage.Headers.Add("Authorization", new string[] { authorizationHeader });
            }
        }
        private async Task<HttpResponseMessage> DoPostPutAsync<T>(
            HttpMethod method,
            string uri,
            T item,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            var request = new HttpRequestMessage(method, uri);

            this.SetAuthorizationHeader(request);

            request.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(authorizationMethod))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                request.Headers.Add("x-requestid", requestId);
            }

            HttpResponseMessage response = await this._httpClient.SendAsync(request);

            //给断路器抛出固定异常
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }
    }
}
