using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Ordering.Domain.SeedWork;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    /// <summary>
    /// 订单仓库
    /// </summary>
    public class OrderRepository
        : IOrderRepository
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public Order Add(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
