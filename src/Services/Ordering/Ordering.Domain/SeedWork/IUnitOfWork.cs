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
        /// 持久化保存所有改变
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 保存所有实体（包括持久化和发布领域事件）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
