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
        /// <summary>
        /// 表示一个订阅事件的方法
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        /// <summary>
        /// 表示一个订阅动态事件的方法
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 摘要：
        ///     表示一个取消订阅事件的方法
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        /// <summary>
        /// 表示一个取消动态事件订阅的方法
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 摘要：
        ///     表示一个发布事件的方法
        /// </summary>
        /// <param name="event">事件类型</param>
        void Publish(IntegrationEvent @event);
    }
}
