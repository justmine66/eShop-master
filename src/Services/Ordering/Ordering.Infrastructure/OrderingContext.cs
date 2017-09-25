using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure.EntityConfigrations;
using Ordering.Infrastructure.Idempotency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    /// <summary>
    /// 订单域上下文
    /// </summary>
    public class OrderingContext
        : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "ordering";//订单子域--数据库默认架构

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PaymentMethod> Payments { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        private readonly IMediator _mediator;
        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }
        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator)
            : base(options)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            System.Diagnostics.Debug.WriteLine("OrderingContext::ctor ->" + this.GetHashCode());
        }
        public static OrderingContext CreateForEFDesignTools(DbContextOptions<OrderingContext> options)
        {
            return new OrderingContext(options);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //应用实体表结构配置信息
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
        }

        /// <summary>
        /// 保存所有实体
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>是否保存成功</returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //001 发布领域事件(最终一致性)
            await this._mediator.DispatchDomainEventAsync(this);
            //002 持久化数据到数据库
            var result = await base.SaveChangesAsync();

            return true;
        }

    }
}
