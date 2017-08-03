using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API
{
    /// <summary>
    /// 目录应用程序配置信息
    /// </summary>
    public class CatalogSettings
    {
        /// <summary>
        /// 事件总线连接字符串，一般为消息服务器主机名或IP地址。
        /// </summary>
        public string EventBusConnection { get; set; }

        /// <summary>
        /// 图片基地址
        /// </summary>
        public string PicBaseUrl { get; set; }

        /// <summary>
        /// 使用自定义数据
        /// </summary>
        public bool UseCustomizationData { get; set; }

        /// <summary>
        /// 启动Azure存储
        /// </summary>
        public bool AzureStorageEnabled { get; set; }
    }
}
