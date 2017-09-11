using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Runtime.Serialization;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 订单取消命令
    /// </summary>
    public class CancelOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public int OrderNumber { get; private set; }

        /// <summary>
        /// 初始化一个订单取消命令
        /// </summary>
        /// <param name="orderNumber"></param>
        public CancelOrderCommand(int orderNumber)
        {
            this.OrderNumber = orderNumber;
        }
    }
}
