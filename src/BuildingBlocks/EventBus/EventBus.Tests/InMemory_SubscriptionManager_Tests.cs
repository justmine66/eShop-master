using System;
using System.Linq;
using Xunit;

namespace EventBus.Tests
{
    /// <summary>
    /// 内存订阅管理器测试
    /// </summary>
    public class InMemory_SubscriptionManager_Tests
    {
        //初始化订阅管理器后，订阅信息应该为空。
        [Fact]
        public void After_Creation_Should_be_Empty()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            Assert.True(manager.IsEmpty);
        }

        //订阅一个事件后，订阅管理器应该包含这个事件。
        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();

            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());

            manager.AddDynamicSubscription<TestDynamicIntegrationEventHandler>(nameof(TestIntegrationEvent));
            Assert.True(manager.HasSubscriptionsForEvent(nameof(TestIntegrationEvent)));
        }

        //移除所有的订阅信息后，订阅信息中事件应该不存在。
        [Fact]
        public void After_One_Subscription_Are_Removed_Event_Shoule_No_Longer_Exists()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();

            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.False(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());

            manager.AddDynamicSubscription<TestDynamicIntegrationEventHandler>(nameof(TestIntegrationEvent));
            manager.RemoveDynamicSubscription<TestDynamicIntegrationEventHandler>(nameof(TestIntegrationEvent));
            Assert.False(manager.HasSubscriptionsForEvent(nameof(TestIntegrationEvent)));
        }

        //删除最后一个订阅信息后，应该引发删除事件。
        [Fact]
        public void After_Deleting_Last_Subscription_Should_Raise_OnEventRemoved()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();

            var raised = false;
            manager.OnEventRemoved += (sender, eventArgs) => raised = true;
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(raised);
        }

        //得到处理事件应该返回所有处理程序
        [Fact]
        public void Get_Handlers_For_Event_Should_Return_All_Handlers()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();

            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.AddDynamicSubscription<TestDynamicIntegrationEventHandler>(nameof(TestIntegrationEvent));

            var handlers = manager.GetHandlersForEvent<TestIntegrationEvent>();
            Assert.Equal(2, handlers.Count());
        }
    }
}
