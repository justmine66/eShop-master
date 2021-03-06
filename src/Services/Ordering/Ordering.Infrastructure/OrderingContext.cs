﻿using MediatR;
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

        #region [ 订单聚合 ]

        /// <summary>
        /// 订单表
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// 订单明细项表
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// 订单状态表
        /// </summary>
        public DbSet<OrderStatus> OrderStatus { get; set; }

        #endregion

        #region [ 买家聚合 ]

        /// <summary>
        /// 买家表
        /// </summary>
        public DbSet<Buyer> Buyers { get; set; }

        /// <summary>
        /// 付款方式表
        /// </summary>
        public DbSet<PaymentMethod> Payments { get; set; }

        /// <summary>
        /// 银行卡类型表
        /// </summary>
        public DbSet<CardType> CardTypes { get; set; }

        #endregion

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
            //注意:这里需要保证业务操作和事件发布的最终一致性
            //001 持久化数据到数据库
            var result = await base.SaveChangesAsync();
            //002 发布领域事件
            if (result > 0)
            {
                await this._mediator.DispatchDomainEventAsync(this);
            }

            return true;
        }
    }
}
