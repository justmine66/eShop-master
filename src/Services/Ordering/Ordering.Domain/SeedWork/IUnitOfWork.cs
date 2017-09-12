using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// 统一工作单元接口,保证聚合根(业务规则和数据)一致性.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 异步保存变化
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 保存所有实体
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
