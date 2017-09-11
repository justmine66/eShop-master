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
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; }

        /// <summary>
        /// 初始化一个 订单状态已改变为已确定库存领域事件 实例
        /// </summary>
        /// <param name="orderId">订单标识</param>
        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
            => this.OrderId = orderId;
    }
}
