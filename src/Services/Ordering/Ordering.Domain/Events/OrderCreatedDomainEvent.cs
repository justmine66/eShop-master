using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    /// <summary>
    /// 订单创建领域事件
    /// </summary>
    public class OrderCreatedDomainEvent
        : INotification
    {
        public string UserId { get; private set; }
        public int CardTypeId { get; private set; }
        public string CardNumber { get; private set; }
        public string CardSecurityNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public DateTime CardExpiration { get; private set; }
        public Order Order { get; private set; }

        public OrderCreatedDomainEvent(Order order, string userId,
            int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName,
            DateTime cardExpiration)
        {
            this.Order = order;
            this.UserId = userId;
            this.CardTypeId = cardTypeId;
            this.CardNumber = cardNumber;
            this.CardSecurityNumber = cardSecurityNumber;
            this.CardHolderName = cardHolderName;
            this.CardExpiration = cardExpiration;
        }
    }
}
