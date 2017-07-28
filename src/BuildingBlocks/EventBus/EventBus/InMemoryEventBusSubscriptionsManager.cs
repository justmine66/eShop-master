using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus
{
    /// <summary>
    /// 基于内存的事件总线订阅管理器
    /// </summary>
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;//事件处理字典
        private readonly List<Type> _eventTypes;//事件类型字典

        /// <summary>
        /// 移除事件处理时触发此事件
        /// </summary>
        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => !_handlers.Keys.Any();
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() => _handlers.Clear();

        /// <summary>
        /// 添加订阅
        /// </summary>
        /// <typeparam name="T">事件源类型</typeparam>
        /// <typeparam name="TH">事件处理类型</typeparam>
        /// <param name="handler">生成事件处理对象的委托函数</param>
        public void AddSubscription<T, TH>(Func<TH> handler)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            //var key = GetEventKey<T>();
            //if (!HasSubscriptionsForEvent<T>())
            //{
            //    _handlers.Add(key, new List<Delegate>());
            //}
            //_handlers[key].Add(handler);
            //_eventTypes.Add(typeof(T));
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            //var handlerToRemove = FindHandlerToRemove<T, TH>();
            //if (handlerToRemove != null)
            //{
            //    var key = GetEventKey<T>();
            //    _handlers[key].Remove(handlerToRemove);
            //    if (!_handlers[key].Any())
            //    {
            //        _handlers.Remove(key);
            //        var eventType = _eventTypes.SingleOrDefault(e => e.Name == key);
            //        if (eventType != null)
            //        {
            //            _eventTypes.Remove(eventType);
            //            RaiseOnEventRemoved(eventType.Name);
            //        }
            //    }

            //}
        }

        /// <summary>
        /// 获取特定事件类型的事件处理
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns></returns>
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }
        /// <summary>
        /// 获取特定名称的事件处理
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns></returns>
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved(this, eventName);
            }
        }
        private Delegate FindHandlerToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!HasSubscriptionsForEvent<T>())
            {
                return null;
            }

            var key = GetEventKey<T>();
            foreach (var func in _handlers[key])
            {
                var genericArgs = func.GetType().GetGenericArguments();
                if (genericArgs.SingleOrDefault() == typeof(TH))
                {
                    return func;
                }
            }

            return null;
        }

        /// <summary>
        /// 判断给定的事件类型是否存在事件订阅
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>是否存在事件订阅</returns>
        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        /// <summary>
        /// 根据指定的事件名判断是否有订阅的事件处理
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        /// <summary>
        /// 根据名称获取事件类型
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public Type GetEventTypeByName(string eventName) => _eventTypes.Single(t => t.Name == eventName);

        /// <summary>
        /// 根据事件类型获取事件标识
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件标识</returns>
        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// 动态添加订阅事件
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 动态移除事件订阅
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!this.HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }
        }
    }
}
