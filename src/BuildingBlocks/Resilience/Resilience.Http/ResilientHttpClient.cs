using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Resilience.Http
{
    /// <summary>
    /// 弹性HttpClient
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
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

        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync(
            string uri,
            string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
        {
            throw new NotImplementedException();
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
    }
}
