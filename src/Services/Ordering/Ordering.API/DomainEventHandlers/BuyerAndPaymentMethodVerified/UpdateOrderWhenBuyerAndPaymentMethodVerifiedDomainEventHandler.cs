using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.Events;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Ordering.API.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
        : IAsyncNotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 领域逻辑：
        /// 当买家的支付方式已经被创建和验证后,然后更新订单对应的BuyerId和PaymentId.
        /// </summary>
        /// <param name="buyerPaymentMethodVerifiedEvent"></param>
        /// <returns></returns>
        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent buyerPaymentMethodVerifiedEvent)
        {
            var orderToUpdate = await this._orderRepository.GetAsync(buyerPaymentMethodVerifiedEvent.OrderId);
            orderToUpdate.SetBuyerId(buyerPaymentMethodVerifiedEvent.Buyer.Id);
            orderToUpdate.SetPaymentId(buyerPaymentMethodVerifiedEvent.PaymentMethod.Id);

            _logger.CreateLogger(nameof(UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler))
                .LogTrace($"Order with Id: {buyerPaymentMethodVerifiedEvent.OrderId} has been successfully updated with a payment method id: { buyerPaymentMethodVerifiedEvent.PaymentMethod.Id }");
        }
    }
}
