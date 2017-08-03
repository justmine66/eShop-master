using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    /// <summary>
    /// 订单状态更改为已支付事件
    /// </summary>
    public class OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }

        public OrderStatusChangedToPaidIntegrationEvent(int orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }
}
