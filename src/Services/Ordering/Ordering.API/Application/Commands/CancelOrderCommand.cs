using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Runtime.Serialization;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 取消订单命令
    /// </summary>
    public class CancelOrderCommand : IRequest<bool>
    {
        [DataMember]
        public int OrderNumber { get; private set; }

        public CancelOrderCommand(int orderNumber)
        {
            this.OrderNumber = orderNumber;
        }
    }
}
