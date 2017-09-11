using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    /// <summary>
    /// 订单状态已改变为已确定库存领域事件
    /// </summary>
    public class OrderStatusChangedToStockConfirmedDomainEvent
        :INotification
    {
        public int OrderId { get; }

        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
            => this.OrderId = orderId;
    }
}
