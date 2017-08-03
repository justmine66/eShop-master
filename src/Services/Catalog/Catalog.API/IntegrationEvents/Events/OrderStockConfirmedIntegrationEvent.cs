using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    /// <summary>
    /// 订单库存已确认事件
    /// </summary>
    public class OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; }
        public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}
