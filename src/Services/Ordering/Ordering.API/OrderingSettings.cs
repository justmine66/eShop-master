using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API
{
    /// <summary>
    /// 订单子域配置信息
    /// </summary>
    public class OrderingSettings
    {
        /// <summary>
        /// 是否使用用户自定义数据
        /// </summary>
        public bool UseCustomizationData { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 事件总线连接
        /// </summary>
        public string EventBusConnection { get; set; }

        /// <summary>
        /// 宽限期时间
        /// </summary>
        public int GracePeriodTime { get; set; }

        /// <summary>
        /// 检查更新时间
        /// </summary>
        public int CheckUpdateTime { get; set; }
    }
}
