using System;
using System.Linq;
using Xunit;

namespace EventBus.Tests
{
    /// <summary>
    /// �ڴ涩�Ĺ���������
    /// </summary>
    public class InMemory_SubscriptionManager_Tests
    {
        //��ʼ�����Ĺ������󣬶�����ϢӦ��Ϊ�ա�
        [Fact]
        public void After_Creation_Should_be_Empty()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            Assert.True(manager.IsEmpty);
        }

        //����һ���¼��󣬶��Ĺ�����Ӧ�ð�������¼���
        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();

            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());

            manager.AddDynamicSubscription<TestDynamicIntegrationEventHandler>(nameof(TestIntegrationEvent));
            Assert.True(manager.HasSubscriptionsForEvent(nameof(TestIntegrationEvent)));
        }

        //�Ƴ����еĶ�����Ϣ�󣬶�����Ϣ���¼�Ӧ�ò����ڡ�
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

        //ɾ�����һ��������Ϣ��Ӧ������ɾ���¼���
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

        //�õ������¼�Ӧ�÷������д������
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
