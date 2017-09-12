using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    /// <summary>
    /// 宽限期已确认的订单集成事件
    /// </summary>
    public class GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}
