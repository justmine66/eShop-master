using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    /// <summary>
    /// 订单上下文
    /// </summary>
    public class OrderingContext
        : DbContext, IUnitOfWork
    {
        const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        private readonly IMediator _mediator;

        /// <summary>
        /// 保存所有实体
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>是否保存成功</returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await this._mediator.DispatchDomainEventAsync(this);

            var result = await base.SaveChangesAsync();

            return true;
        }
    }
}
