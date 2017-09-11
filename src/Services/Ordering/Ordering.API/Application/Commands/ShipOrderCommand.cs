using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 派送订单命令
    /// </summary>
    public class ShipOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public int OrderNumber { get; private set; }

        /// <summary>
        /// 初始化一个派送订单命令实例
        /// </summary>
        /// <param name="orderNumber">订单号</param>
        public ShipOrderCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
