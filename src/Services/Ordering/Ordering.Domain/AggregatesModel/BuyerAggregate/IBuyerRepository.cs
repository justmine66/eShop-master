using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    /// <summary>
    /// 买家仓储
    /// </summary>
    public interface IBuyerRepository : IRepository<Buyer>
    {
        /// <summary>
        /// 新增买家
        /// </summary>
        /// <param name="buyer">买家</param>
        /// <returns>买家</returns>
        Buyer Add(Buyer buyer);

        /// <summary>
        /// 更新买家
        /// </summary>
        /// <param name="buyer">买家</param>
        /// <returns>买家</returns>
        Buyer Update(Buyer buyer);

        /// <summary>
        /// 根据身份标识查找买家,异步任务.
        /// </summary>
        /// <param name="buyerIdentityGuid">买家身份标识</param>
        /// <returns>买家</returns>
        Task<Buyer> FindAsync(string buyerIdentityGuid);
    }
}
