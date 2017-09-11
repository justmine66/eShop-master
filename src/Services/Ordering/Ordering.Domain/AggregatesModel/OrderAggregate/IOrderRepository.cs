using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 订单仓储
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// 新增订单
        /// </summary>
        /// <param name="order">订单</param>
        /// <returns>新增的订单</returns>
        Order Add(Order order);
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="order">订单</param>
        void Update(Order order);
        /// <summary>
        /// 获取订单,异步任务
        /// </summary>
        /// <param name="id">订单标识</param>
        /// <returns></returns>
        Task<Order> GetAsync(int id);
    }
}
