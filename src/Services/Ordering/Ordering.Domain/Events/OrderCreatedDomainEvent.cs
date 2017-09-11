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
        /// <summary>
        /// 用户标识
        /// </summary>
        public string UserId { get; private set; }
        /// <summary>
        /// 银行卡类型标识
        /// </summary>
        public int CardTypeId { get; private set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNumber { get; private set; }
        /// <summary>
        /// 支付密码
        /// </summary>
        public string CardSecurityNumber { get; private set; }
        /// <summary>
        /// 持卡人姓名
        /// </summary>
        public string CardHolderName { get; private set; }
        /// <summary>
        /// 银行卡过期时间
        /// </summary>
        public DateTime CardExpiration { get; private set; }
        /// <summary>
        /// 订单
        /// </summary>
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
