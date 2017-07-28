using System;
using EventBus.Events;
using IntegrationEventLogEF.ValueObjects;
using Newtonsoft.Json;

namespace IntegrationEventLogEF
{
    /// <summary>
    /// 一体化事件日志入口实体
    /// </summary>
    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }
        public IntegrationEventLogEntry(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }
        /// <summary>
        /// 事件标识
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// 事件类型名称
        /// </summary>
        public string EventTypeName { get; private set; }

        /// <summary>
        /// 事件发布状态
        /// </summary>
        public EventStateEnum State { get; set; }

        /// <summary>
        /// 已发送事件次数
        /// </summary>
        public int TimesSent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; private set; }
    }
}
