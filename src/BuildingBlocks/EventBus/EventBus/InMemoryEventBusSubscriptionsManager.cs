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
    /// 内存事件总线订阅管理器
    /// </summary>
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;//事件处理字典
        private readonly List<Type> _eventTypes;//事件类型字典

        /// <summary>
        /// 移除事件的触发事件
        /// </summary>
        public event EventHandler<string> OnEventRemoved;

        /// <summary>
        /// 初始化内存事件订阅管理器实例
        /// </summary>
        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        /// <summary>
        /// 事件处理是否为空
        /// </summary>
        public bool IsEmpty => !_handlers.Keys.Any();
        /// <summary>
        /// 清空事件处理
        /// </summary>
        public void Clear() => _handlers.Clear();

        /// <summary>
        /// 获取特定事件类型的事件处理集
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件处理集</returns>
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        /// <summary>
        /// 获取特定名称的事件处理集
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <returns>事件处理集</returns>
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

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
        /// 添加动态订阅事件
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            this.DoAddSubscription(typeof(TH), eventName, isDynamic: true);
        }

        /// <summary>
        /// 添加订阅事件
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            this.DoAddSubscription(typeof(TH), eventName, isDynamic: false);
            _eventTypes.Add(typeof(T));
        }

        /// <summary>
        /// 移除动态事件订阅
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = this.FindDynamicSubscriptionToRemove<TH>(eventName);
            this.DoRemoveHandler(eventName, handlerToRemove);
        }

        /// <summary>
        /// 移除事件订阅
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        public void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent
        {
            var eventName = this.GetEventKey<T>();
            var handlerToRemove = this.FindDynamicSubscriptionToRemove<TH>(eventName);
            this.DoRemoveHandler(eventName, handlerToRemove);
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            //1、如果该事件不存在订阅。
            if (!this.HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }
            //2、如果该事件存在订阅
            //2.1 防止重复添加订阅
            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'",
                    nameof(handlerType));
            }
            //2.2 添加订阅
            if (isDynamic)
            {
                _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            }
            else
            {
                _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            }
        }

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
        {
            return this.DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return this.DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            //1.不存在事件订阅，返回null。
            if (!this.HasSubscriptionsForEvent(eventName)) return null;
            //2.存在事件订阅
            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                //1、移除事件处理
                this._handlers[eventName].Remove(subsToRemove);
                //2、移除事件
                if (!this._handlers[eventName].Any())
                {
                    //2.1 移除事件处理键
                    _handlers.Remove(eventName);
                    //2.2 移除事件类型
                    var eventType = this._eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (null != eventType) this._eventTypes.Remove(eventType);
                    //2.3 触发移除事件，通知外部应用程序。
                    this.RaiseOnEventRemoved(eventName);
                }
            }
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null) OnEventRemoved(this, eventName);
        }
    }
}
