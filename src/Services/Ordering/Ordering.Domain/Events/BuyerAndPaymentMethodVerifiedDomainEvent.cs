using MediatR;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    /// <summary>
    /// 买家支付方式已审核领域事件
    /// </summary>
    public class BuyerAndPaymentMethodVerifiedDomainEvent
        : INotification
    {
        /// <summary>
        /// 买家
        /// </summary>
        public Buyer Buyer { get; private set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public PaymentMethod PaymentMethod { get; private set; }
        /// <summary>
        /// 订单标识
        /// </summary>
        public int OrderId { get; private set; }

        /// <summary>
        /// 初始化一个买家支付方式已审核领域事件实例
        /// </summary>
        /// <param name="buyer">买家</param>
        /// <param name="payment">支付方式</param>
        /// <param name="orderId">订单标识</param>
        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, int orderId)
        {
            this.Buyer = buyer;
            this.PaymentMethod = payment;
            this.OrderId = orderId;
        }
}
}
