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
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<Delegate>> _handlers;//事件处理字典
        private readonly List<Type> _eventTypes;//事件字典

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<Delegate>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
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
            var key = GetEventKey<T>();
            if (!HasSubscriptionsForEvent<T>())
            {
                _handlers.Add(key, new List<Delegate>());
            }
            _handlers[key].Add(handler);
            _eventTypes.Add(typeof(T));
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
            var handlerToRemove = FindHandlerToRemove<T, TH>();
            if (handlerToRemove != null)
            {
                var key = GetEventKey<T>();
                _handlers[key].Remove(handlerToRemove);
                if (!_handlers[key].Any())
                {
                    _handlers.Remove(key);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == key);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                        RaiseOnEventRemoved(eventType.Name);
                    }
                }

            }
        }

        /// <summary>
        /// 获取特定事件类型的事件处理
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns></returns>
        public IEnumerable<Delegate> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }
        /// <summary>
        /// 获取特定名称的事件处理
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns></returns>
        public IEnumerable<Delegate> GetHandlersForEvent(string eventName) => _handlers[eventName];

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
        /// 判断给定的事件名称是否存在事件订阅
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

        private string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
    }
}
