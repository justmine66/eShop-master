using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    /// <summary>
    /// 订单状态更改为等待验证事件
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }
        public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderId,
            IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }

    /// <summary>
    /// 订单库存项
    /// </summary>
    public class OrderStockItem
    {
        /// <summary>
        /// 商品标识
        /// </summary>
        public int ProductId { get; }

        /// <summary>
        /// 订单中单位库存累积量
        /// </summary>
        public int Units { get; }

        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }
    }
}
