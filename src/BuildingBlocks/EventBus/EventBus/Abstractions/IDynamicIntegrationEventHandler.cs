using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Abstractions
{
    /// <summary>
    /// 一体化动态事件处理接口
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        /// <summary>
        /// 事件处理的异步任务
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <returns>任务</returns>
        Task Handle(dynamic eventData);
    }
}
