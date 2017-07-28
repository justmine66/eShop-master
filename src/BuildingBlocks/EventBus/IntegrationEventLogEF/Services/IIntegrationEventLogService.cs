using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using EventBus.Events;

namespace IntegrationEventLogEF.Services
{
    /// <summary>
    /// 一体化事件日志服务
    /// </summary>
    public interface IIntegrationEventLogService
    {
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <param name="transaction">事务</param>
        /// <returns></returns>
        Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction);

        /// <summary>
        /// 标志事件为已发布
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
    }
}
