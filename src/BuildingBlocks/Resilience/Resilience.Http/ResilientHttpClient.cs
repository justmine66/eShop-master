using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Resilience.Http
{
    /// <summary>
    /// 弹性HttpClient（根据配置策略，弹性处理请求部分失败）。
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        #region [ 私有字段和构造函数重载 ]

        readonly HttpClient _client;
        readonly ILogger<ResilientHttpClient> _logger;
        readonly Func<string, IEnumerable<Policy>> _policyCreator;
        ConcurrentDictionary<string, PolicyWrap> _policyWrappers;
        readonly IHttpContextAccessor _httpContextAccessor;

        public ResilientHttpClient(
            Func<string, IEnumerable<Policy>> policyCreator,
            ILogger<ResilientHttpClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            this._client = new HttpClient();
            this._logger = logger;
            this._policyCreator = policyCreator;
            this._policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(HttpContextAccessor));
        }

        #endregion

        #region [ 公共方法 ]

        public Task<HttpResponseMessage> DeleteAsync(
            string uri,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

                SetAuthorizationHeader(requestMessage);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                return await _client.SendAsync(requestMessage);
            });
        }

        public Task<string> GetStringAsync(
            string uri,
            string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            string origin = this.GetOriginFromUri(uri);

            return this.HttpInvoker<string>(origin, async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                this.SetAuthorizationHeader(request);

                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                var response = await this._client.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            });
        }

        public Task<HttpResponseMessage> PostAsync<T>(
            string uri,
            T item,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return this.DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        public Task<HttpResponseMessage> PutAsync<T>(
            string uri,
            T item,
            string authorizationToken = null,
            string requestId = null,
            string authorizationMethod = "Bearer")
        {
            return this.DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
        }

        #endregion

        #region [ 内部方法 ]

        private Task<HttpResponseMessage> DoPostPutAsync<T>(
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

            var origin = GetOriginFromUri(uri);

            return HttpInvoker(origin, async () =>
            {
                var requestMessage = new HttpRequestMessage(method, uri);

                SetAuthorizationHeader(requestMessage);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                if (requestId != null)
                {
                    requestMessage.Headers.Add("x-requestid", requestId);
                }

                var response = await _client.SendAsync(requestMessage);

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = this.NormolizeOrigin(origin);

            if (!this._policyWrappers.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(this._policyCreator(normalizedOrigin).ToArray());
                this._policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            return await policyWrap.ExecuteAsync<T>(action, new Context(normalizedOrigin));
        }

        private string GetOriginFromUri(string uriString)
        {
            var uri = new Uri(uriString);

            var origin = $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";

            return origin;
        }

        private string NormolizeOrigin(string origin)
        {
            return origin?.Trim()?.ToLower();
        }

        private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            var authorizationHeader = this._httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
        }

        #endregion
    }
}
