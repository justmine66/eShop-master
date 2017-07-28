using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEventLogEF.ValueObjects
{
    /// <summary>
    /// 事件发布状态枚举
    /// </summary>
    public enum EventStateEnum
    {
        /// <summary>
        /// 未发布
        /// </summary>
        NotPublished = 0,
        /// <summary>
        /// 已发布
        /// </summary>
        Published = 1,
        /// <summary>
        /// 发布失败
        /// </summary>
        PublishedFailed = 2
    }
}
