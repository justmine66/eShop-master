using System;
using System.Linq;
using Xunit;

namespace EventBus.Test
{
    /// <summary>
    /// �ڴ�-���Ĺ�����-��Ԫ����
    /// </summary>
    public class InMemory_SubscriptionManager_Tests
    {
        /// <summary>
        /// ���Գ�ʼ���¼����ߺ��¼�����Ӧ��Ϊ�ա�
        /// </summary>
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            Assert.True(manager.IsEmpty);
        }

        /// <summary>
        /// �������һ���¼�����
        /// </summary>
        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>(() => new TestIntegrationEventHandler());
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        /// <summary>
        /// �����Ƴ����еĶ���
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
        /// ����ɾ���¼����Ƿ�����֪ͨ��
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
        /// ����-��ȡ�¼����Ͷ��ĵ������¼�����
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
