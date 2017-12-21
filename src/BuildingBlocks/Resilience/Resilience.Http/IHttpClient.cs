using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Resilience.Http
{
    /// <summary>
    /// HttpClient 接口
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 获取结果字符串
        /// </summary>
        /// <param name="uri">请求Uri</param>
        /// <param name="authorizationToken">授权令牌</param>
        /// <param name="authorizationMethod">授权方案</param>
        /// <returns></returns>
        Task<string> GetStringAsync(
            string uri, 
            string authorizationToken = null, 
            string authorizationMethod = "Bearer");

        /// <summary>
        /// 请求执行一个创建操作
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="uri">请求Uri</param>
        /// <param name="item">资源信息</param>
        /// <param name="authorizationToken">授权令牌</param>
        /// <param name="requestId">请求标识</param>
        /// <param name="authorizationMethod">授权方案</param>
        /// <returns></returns>
        Task<HttpResponseMessage> PostAsync<T>(
            string uri, 
            T item, 
            string authorizationToken = null, 
            string requestId = null, 
            string authorizationMethod = "Bearer");

        /// <summary>
        /// 请求执行一个删除操作
        /// </summary>
        /// <param name="uri">请求Uri</param>
        /// <param name="authorizationToken">授权令牌</param>
        /// <param name="requestId">请求标识</param>
        /// <param name="authorizationMethod">授权方案</param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteAsync(
            string uri, 
            string authorizationToken = null, 
            string requestId = null, 
            string authorizationMethod = "Bearer");

        /// <summary>
        /// 请求执行一个更新操作
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="uri">请求Uri</param>
        /// <param name="item">资源信息</param>
        /// <param name="authorizationToken">授权令牌</param>
        /// <param name="requestId">请求标识</param>
        /// <param name="authorizationMethod">授权方案</param>
        /// <returns></returns>
        Task<HttpResponseMessage> PutAsync<T>(
            string uri, 
            T item, 
            string authorizationToken = null, 
            string requestId = null, 
            string authorizationMethod = "Bearer");
    }
}
