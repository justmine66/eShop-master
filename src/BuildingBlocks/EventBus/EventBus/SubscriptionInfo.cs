using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    /// <summary>
    /// 基于内存的事件总线订阅管理器
    /// </summary>
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 订阅信息
        /// </summary>
        public class SubscriptionInfo
        {
            /// <summary>
            /// 是否动态
            /// </summary>
            public bool IsDynamic { get; }

            /// <summary>
            /// 事件处理类型
            /// </summary>
            public Type HandlerType { get; }

            /// <summary>
            /// 初始化订阅信息实例
            /// </summary>
            /// <param name="isDynamic">是否动态</param>
            /// <param name="handlerType">事件类型</param>
            private SubscriptionInfo(bool isDynamic, Type handlerType)
            {
                this.IsDynamic = isDynamic;
                this.HandlerType = handlerType;
            }

            /// <summary>
            /// 初始化动态订阅信息实例
            /// </summary>
            /// <param name="handlerType">事件类型</param>
            /// <returns>订阅信息实例</returns>
            public static SubscriptionInfo Dynamic(Type handlerType)
            {
                return new SubscriptionInfo(true, handlerType);
            }

            /// <summary>
            /// 初始化类型订阅信息实例
            /// </summary>
            /// <param name="handlerType">事件类型</param>
            /// <returns>订阅信息实例</returns>
            public static SubscriptionInfo Typed(Type handlerType)
            {
                return new SubscriptionInfo(false, handlerType);
            }
        }
    }
}
