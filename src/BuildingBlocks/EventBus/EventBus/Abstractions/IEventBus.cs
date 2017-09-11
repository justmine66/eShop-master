using System;
using System.Collections.Generic;
using System.Text;
using EventBus.Events;

namespace EventBus.Abstractions
{
    /// <summary>
    /// 摘要：
    ///     事件总线。
    /// 说明：
    ///     集中式事件处理中心。
    /// </summary>
    public interface IEventBus
    {
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        void Publish(IntegrationEvent @event);
    }
}
