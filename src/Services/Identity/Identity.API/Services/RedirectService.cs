using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// 重定向服务
    /// </summary>
    public class RedirectService : IRedirectService
    {
        /// <summary>
        /// 从返回URL提取重定向URI
        /// </summary>
        /// <param name="url">返回URL</param>
        /// <returns>重定向URI</returns>
        public string ExtractRedirectUriFromReturnUrl(string url)
        {
            var result = "";
            var decodedUrl = System.Net.WebUtility.HtmlDecode(url);
            var results = Regex.Split(decodedUrl, "redirect_uri=");
            if (results.Length < 2)
                return "";

            result = results[1];

            var splitKey = "";
            if (result.Contains("signin-oidc"))
                splitKey = "signin-oidc";
            else
                splitKey = "scope";
            
            results = Regex.Split(result, splitKey);
            if (results.Length < 2)
                return "";

            result = results[0];

            return result.Replace("%3A", ":").Replace("%2F", "/").Replace("&", "");
        }
    }
}
