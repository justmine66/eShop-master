using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Abstractions
{
    /// <summary>
    /// 一体化事件动态处理接口
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        /// <summary>
        /// 异步处理任务
        /// </summary>
        /// <param name="eventData">事件数据</param>
        /// <returns>任务</returns>
        Task Handle(dynamic eventData);
    }
}
