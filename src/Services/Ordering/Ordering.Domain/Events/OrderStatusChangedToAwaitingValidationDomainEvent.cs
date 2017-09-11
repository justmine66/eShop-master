using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    /// <summary>
    /// 订单状态已经改变成等待验证领域事件
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationDomainEvent
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
        /// 初始化一个 订单状态已经改变成等待验证领域事件 实例
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItems"></param>
        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems)
        {
            this.OrderId = orderId;
            this.OrderItems = orderItems;
        }
    }
}
