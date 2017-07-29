using System;
using System.Collections.Generic;
using System.Text;
using EventBus.Abstractions;
using EventBus.Events;
using static EventBus.InMemoryEventBusSubscriptionsManager;

namespace EventBus
{
    /// <summary>
    /// 事件总线订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 移除事件的触发事件
        /// </summary>
        event EventHandler<string> OnEventRemoved;

        /// <summary>
        /// 添加动态订阅事件
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        void AddDynamicSubscription<TH>(string eventName)
           where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 移除事件订阅
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;

        /// <summary>
        /// 动态移除事件订阅
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 根据指定的事件类型判断是否有订阅的事件处理
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>是否有订阅的事件处理</returns>
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        /// <summary>
        /// 根据指定的事件名判断是否有订阅的事件处理
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>是否有订阅的事件处理</returns>
        bool HasSubscriptionsForEvent(string eventName);

        /// <summary>
        /// 根据事件名，获取指定事件类型
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>事件类型</returns>
        Type GetEventTypeByName(string eventName);

        /// <summary>
        /// 清空
        /// </summary>
        void Clear();

        /// <summary>
        /// 根据事件类型，获取事件处理集。
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件处理集</returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;

        /// <summary>
        /// 根据事件名，获取事件处理集。
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>事件处理集</returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        /// <summary>
        /// 根据事件类型获取事件标识
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件标识</returns>
        string GetEventKey<T>();
    }
}
