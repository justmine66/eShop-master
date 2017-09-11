using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.Idempotency
{
    /// <summary>
    /// 客户端请求
    /// </summary>
    public class ClientRequest
    {
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
