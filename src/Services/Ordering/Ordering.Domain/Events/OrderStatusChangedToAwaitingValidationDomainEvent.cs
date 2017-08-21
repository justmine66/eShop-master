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
        public int OrderId { get; }
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems)
        {
            this.OrderId = orderId;
            this.OrderItems = orderItems;
        }
    }
}
