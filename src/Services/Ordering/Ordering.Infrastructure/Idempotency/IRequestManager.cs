using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Idempotency
{
    /// <summary>
    /// 请求管理接口
    /// </summary>
    public interface IRequestManager
    {
        /// <summary>
        /// 指定标识的请求是否存在
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistAsync(Guid id);

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <typeparam name="T">请求实体</typeparam>
        /// <param name="id">标识</param>
        /// <returns></returns>
        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}
