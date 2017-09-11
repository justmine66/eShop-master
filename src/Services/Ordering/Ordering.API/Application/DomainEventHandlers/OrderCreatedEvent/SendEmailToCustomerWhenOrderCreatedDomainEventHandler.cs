using MediatR;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.OrderCreatedEvent
{
    /// <summary>
    /// 当处理订单创建领域事件时，发送邮件给客户。
    /// </summary>
    public class SendEmailToCustomerWhenOrderCreatedDomainEventHandler 
        : IAsyncNotificationHandler<OrderCreatedDomainEvent>
    {
        public Task Handle(OrderCreatedDomainEvent notification)
        {
            //todo 发送邮件逻辑
            throw new NotImplementedException();
        }
    }
}
