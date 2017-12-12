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
using Autofac;
using Newtonsoft.Json.Linq;

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
        private readonly ILifetimeScope _autofac;//_autofac作用域
        private readonly string AUTOFAC_SCOPE_NAME = "eshop_event_bus";//_autofac作用域名称
        private readonly int _retryCount;//重试次数

        private IModel _consumerChannel;//信道
        private string _queueName;//队列名称

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
            ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac,
            IEventBusSubscriptionsManager subsManager,
            int retryCount = 5)
        {
            _persistentConnection = persistentConnection ??
                throw new ArgumentNullException(nameof(persistentConnection));//Rabbit持久连接器
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));//日志记录器
            _subsManager = subsManager ??
                new InMemoryEventBusSubscriptionsManager();//订阅管理器
            _consumerChannel = CreateConsumerChannel();//创建实时消费信道。
            _autofac = autofac;

            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;//注册移除事件回调。

            this._retryCount = retryCount;
        }

        /// <summary>
        /// 表示一个订阅事件的方法
        /// </summary>
        /// <typeparam name="T">事件</typeparam>
        /// <typeparam name="TH">事件处理</typeparam>
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            //1、建立消息队列通信机制
            this.DoInternalSubscription(eventName);
            //2、添加订阅
            _subsManager.AddSubscription<T, TH>();
        }

        /// <summary>
        /// 表示一个订阅动态事件的方法
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            //1、建立消息队列通信机制
            this.DoInternalSubscription(eventName);
            //2、添加订阅
            _subsManager.AddDynamicSubscription<TH>(eventName);
        }

        /// <summary>
        /// 表示一个取消动态事件订阅的方法
        /// </summary>
        /// <typeparam name="TH">事件处理</typeparam>
        /// <param name="eventName">事件名</param>
        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
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

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IntegrationEvent @event)
        {
            //1、保证连接是否有效
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            //2、声明事件发布重试策略，处理瞬时或不确定移除导致失败的情况。
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(this._retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex.ToString());
                });
            //3、发布事件到消息队列
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

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                //保证RabbitMQ连接
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }
                //建立从交换机绑定事件消息机制
                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                                      exchange: BROKER_NAME,
                                      routingKey: eventName);
                }
            }
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
            //1、保证IRabbitMQ处于连接状态
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            //2、创建信道
            var channel = _persistentConnection.CreateModel();
            //2.1 声明交换机
            channel.ExchangeDeclare(exchange: BROKER_NAME,
                                 type: "direct");
            //2.2 声明消息队列
            _queueName = channel.QueueDeclare().QueueName;
            //3、建立消费机制
            //3.1 初始化消费实例
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);
                //处理队列信息，即发布事件的信息。
                await ProcessEvent(eventName, message);
            };
            //3.2 启动消费
            channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);
            //3.3 异常回调处理
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            //判断是否存在事件处理
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var intergrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handler").Invoke(handler, new object[] { intergrationEvent });
                        }
                    }
                }
            }
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            //1、保证RabbitMQ已连接
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            //2、建立从交换机绑定消息机制
            using (var channel = _persistentConnection.CreateModel())//创建通道
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);//根据路由规则(事件名称)，从交换机绑定队列消息。
                //订阅管理器，是否清空判断。
                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }
    }
}
