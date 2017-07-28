using System;
using System.Linq;
using Xunit;

namespace EventBus.Test
{
    /// <summary>
    /// 内存-订阅管理器-单元测试
    /// </summary>
    public class InMemory_SubscriptionManager_Tests
    {
        /// <summary>
        /// 测试初始化事件总线后，事件处理应该为空。
        /// </summary>
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            Assert.True(manager.IsEmpty);
        }

        /// <summary>
        /// 测试添加一个事件订阅
        /// </summary>
        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>(() => new TestIntegrationEventHandler());
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        /// <summary>
        /// 测试移除所有的订阅
        /// </summary>
        [Fact]
        public void After_All_Subscriptions_Are_Deleted_Event_Should_No_Longer_Exists()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>(() => new TestIntegrationEventHandler());
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.False(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        /// <summary>
        /// 测试删除事件，是否会产生通知。
        /// </summary>
        [Fact]
        public void Deleting_Last_Subscription_Should_Raise_On_Deleted_Event()
        {
            bool raised = false;
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.OnEventRemoved += (o, e) => raised = true;
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>(() => new TestIntegrationEventHandler());
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(raised);
        }

        /// <summary>
        /// 测试-获取事件类型订阅的所有事件处理
        /// </summary>
        [Fact]
        public void Get_Handlers_For_Event_Should_Return_All_Handlers()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>(() => new TestIntegrationEventHandler());
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationOtherEventHandler>(() => new TestIntegrationOtherEventHandler());
            var handlers = manager.GetHandlersForEvent<TestIntegrationEvent>();
            Assert.Equal(2, handlers.Count());
        }
    }
}
