using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    /// <summary>
    /// 订单库存拒绝集成事件
    /// </summary>
    public class OrderStockRejectedIntegrationEvent: IntegrationEvent
    {
        public int OrderId { get; }

        public List<ConfirmedOrderStockItem> OrderStockItems { get; }

        public OrderStockRejectedIntegrationEvent(int orderId,
            List<ConfirmedOrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }

    public class ConfirmedOrderStockItem
    {
        public int ProductId;
        public bool HasStock;

        public ConfirmedOrderStockItem(int productId, bool hasStock)
        {
            this.ProductId = productId;
            this.HasStock = hasStock;
        }
    }
}
