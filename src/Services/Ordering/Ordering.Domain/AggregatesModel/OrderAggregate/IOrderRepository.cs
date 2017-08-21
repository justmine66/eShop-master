using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 订单仓库接口
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        Order Add(Order order);
        void Update(Order order);
        Task<Order> GetAsync(int id);
    }
}
