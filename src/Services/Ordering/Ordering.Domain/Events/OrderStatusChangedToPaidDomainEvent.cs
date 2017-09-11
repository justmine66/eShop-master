using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    /// <summary>
    /// 设置订单状态为已支付的领域事件
    /// </summary>
    public class OrderStatusChangedToPaidDomainEvent
        :INotification
    {
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; }
        /// <summary>
        /// 订单项集合
        /// </summary>
        public IEnumerable<OrderItem> OrderItems { get; }

        /// <summary>
        /// 初始化一个 设置订单状态为已支付的领域事件 实例
        /// </summary>
        /// <param name="orderId">订单标识</param>
        /// <param name="orderItems">订单项集合</param>
        public OrderStatusChangedToPaidDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems)
        {
            this.OrderId = orderId;
            this.OrderItems = orderItems;
        }
    }
}
