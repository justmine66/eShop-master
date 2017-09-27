using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Ordering.Domain.SeedWork;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ordering.Infrastructure.Repositories
{
    /// <summary>
    /// 订单仓储
    /// </summary>
    public class OrderRepository
        : IOrderRepository
    {
        private readonly OrderingContext _context;//数据库上下文
        public IUnitOfWork UnitOfWork => this._context;//统一工作单元

        /// <summary>
        /// 初始化一个订单仓储实例
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public OrderRepository(OrderingContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Order Add(Order order)
        {
            return this._context.Add<Order>(order).Entity;
        }

        public async Task<Order> GetAsync(int orderId)
        {
            var order = await this._context.Orders.FindAsync(orderId);
            if (order != null)
            {
                await _context.Entry(order)
                   .Collection(i => i.OrderItems).LoadAsync();
                await _context.Entry(order)
                    .Reference(o => o.OrderStatus).LoadAsync();
                await _context.Entry(order)
                    .Reference(o => o.Address).LoadAsync();
            }

            return order;
        }

        public void Update(Order order)
        {
            this._context.Entry(order).State = EntityState.Modified;
        }
    }
}
