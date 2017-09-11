using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    /// <summary>
    /// 订单状态改变成等待验证集成事件
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; }
        /// <summary>
        /// 订单库存列表
        /// </summary>
        public IEnumerable<OrderStockItem> OrderStockItems { get; }

        /// <summary>
        /// 初始化一个 订单状态改变成等待验证集成事件 实例
        /// </summary>
        /// <param name="orderId">订单标识</param>
        /// <param name="orderStockItems">订单库存列表</param>
        public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }
}
