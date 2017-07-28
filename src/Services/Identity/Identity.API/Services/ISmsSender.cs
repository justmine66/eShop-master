using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// 短信发送服务
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="number">号码</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        Task SendSmsAsync(string number, string message);
    }
}
