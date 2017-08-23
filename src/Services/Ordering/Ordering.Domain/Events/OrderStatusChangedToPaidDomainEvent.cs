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
        public int OrderId { get; }
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToPaidDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems)
        {
            this.OrderId = orderId;
            this.OrderItems = orderItems;
        }
    }
}
