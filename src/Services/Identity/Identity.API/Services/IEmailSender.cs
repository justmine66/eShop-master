using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    /// <summary>
    /// 邮件发送服务
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <param name="subject">主题</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        Task SendEmailAsync(string email, string subject, string message);
    }
}
