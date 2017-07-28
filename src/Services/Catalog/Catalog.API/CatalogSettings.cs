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
        /// 提供外部访问的目录基地址
        /// </summary>
        public string ExternalCatalogBaseUrl { get; set; }
        /// <summary>
        /// 事件总线连接字符串，一般为消息服务器主机名或IP地址。
        /// </summary>
        public string EventBusConnection { get; set; }
    }
}
