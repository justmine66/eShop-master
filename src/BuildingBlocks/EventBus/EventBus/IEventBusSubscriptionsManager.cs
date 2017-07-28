using System;
using System.Collections.Generic;
using System.Text;
using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus
{
    /// <summary>
    /// 事件总线订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T, TH>(Func<TH> handler)
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<Delegate> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<Delegate> GetHandlersForEvent(string eventName);
    }
}
