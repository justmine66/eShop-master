using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace EventBusRabbitMQ
{
    /// <summary>
    /// RabbitMQ持久连接器
    /// </summary>
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// 创建通用AMQP模型
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}
