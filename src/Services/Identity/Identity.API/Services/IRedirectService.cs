using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// 重定向服务
    /// </summary>
    public interface IRedirectService
    {
        /// <summary>
        /// 从返回URL提取重定向URI
        /// </summary>
        /// <param name="url">返回URL</param>
        /// <returns>重定向URI</returns>
        string ExtractRedirectUriFromReturnUrl(string url);
    }
}
