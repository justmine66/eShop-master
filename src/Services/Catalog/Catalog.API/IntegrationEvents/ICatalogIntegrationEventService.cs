using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Events;

namespace Catalog.API.IntegrationEvents
{
    /// <summary>
    /// 目录一体化事件服务
    /// </summary>
    public interface ICatalogIntegrationEventService
    {
        /// <summary>
        /// 保存事件并改变目录上下文的异步任务
        /// </summary>
        /// <param name="">事件</param>
        /// <returns></returns>
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt);

        /// <summary>
        /// 通过事件总线发布事件
        /// </summary>
        /// <param name="evt">事件</param>
        /// <returns></returns>
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
