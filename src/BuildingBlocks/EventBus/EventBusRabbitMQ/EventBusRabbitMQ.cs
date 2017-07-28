using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ
{
    /// <summary>
    /// 摘要：
    ///     基于RabbitMQ实现的事件总线。
    /// 说明：
    ///     基于RabbitMQ提供分布式事件集中式处理的支持。
    /// </summary>
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "eshop_event_bus";//消息代理名称

        private readonly IRabbitMQPersistentConnection _persistentConnection;//持久连接器
        private readonly ILogger<EventBusRabbitMQ> _logger;//日志记录器
        private readonly IEventBusSubscriptionsManager _subsManager;//事件总线订阅管理器


        private IModel _consumerChannel;//信道
        private string _queueName;//队列名称

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger, IEventBusSubscriptionsManager subsManager)
        {
            _persistentConnection = persistentConnection ?? 
                throw new ArgumentNullException(nameof(persistentConnection));//Rabbit持久连接器
            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));//日志记录器
            _subsManager = subsManager ?? 
                new InMemoryEventBusSubscriptionsManager();//订阅管理器
            _consumerChannel = CreateConsumerChannel();//创建实时消费信道。

            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;//注册移除事件回调。
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())//创建通道
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);//根据路由规则(事件名称)，从交换机绑定队列消息。

                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)//判断连接是否有效
                _persistentConnection.TryConnect();

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });//声明事件发布重试策略，处理瞬时或不确定移除导致失败的情况。

            using (var channel = _persistentConnection.CreateModel())//创建信道
            {
                var eventName = @event.GetType()
                    .Name;//事件名称

                channel.ExchangeDeclare(exchange: BROKER_NAME,
                                    type: "direct");//申明交换机
                //将事件对象序列化成（RabbitMQ能传递的）二进制块。
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                //发布事件
                policy.Execute(() =>
                {
                    channel.BasicPublish(exchange: BROKER_NAME,
                                     routingKey: eventName,
                                     basicProperties: null,
                                     body: body);
                });
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="handler"></param>
        public void Subscribe<T, TH>(Func<TH> handler)
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                    _persistentConnection.TryConnect();

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                                      exchange: BROKER_NAME,
                                      routingKey: eventName);
                }
            }

            _subsManager.AddSubscription<T, TH>(handler);

        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        public void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        private static Func<IIntegrationEventHandler> FindHandlerByType(Type handlerType, IEnumerable<Func<IIntegrationEventHandler>> handlers)
        {
            foreach (var func in handlers)
            {
                if (func.GetMethodInfo().ReturnType == handlerType)
                {
                    return func;
                }
            }

            return null;
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }

            _subsManager.Clear();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME,
                                 type: "direct");

            _queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName,
                                 noAck: true,
                                 consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var eventType = _subsManager.GetEventTypeByName(eventName);
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                var handlers = _subsManager.GetHandlersForEvent(eventName);

                foreach (var handlerfactory in handlers)
                {
                    var handler = handlerfactory.DynamicInvoke();
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
    }
}
