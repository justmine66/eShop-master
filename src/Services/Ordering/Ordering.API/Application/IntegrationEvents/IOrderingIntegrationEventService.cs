using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents
{
    /// <summary>
    /// 订单集成事件服务
    /// </summary>
    public interface IOrderingIntegrationEventService
    {
        /// <summary>
        /// 通过事件总线发布
        /// </summary>
        /// <param name="event">领域事件</param>
        /// <returns></returns>
        Task PublishThroughEventBusAsync(IntegrationEvent @event);
    }
}
