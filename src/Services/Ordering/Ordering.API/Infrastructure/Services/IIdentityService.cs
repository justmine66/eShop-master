using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure.Services
{
    /// <summary>
    /// 身份服务
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        /// 获取用户身份
        /// </summary>
        /// <returns>身份标识</returns>
        string GetUserIdentity();
    }
}
