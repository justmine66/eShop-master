using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    /// <summary>
    /// 当处理买家支付方式审核命令时,更新订单.
    /// </summary>
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler 
        : IAsyncNotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent @event)
        {
            var orderToUpdate =await this._orderRepository.GetAsync(@event.OrderId);
            orderToUpdate.SetBuyerId(@event.Buyer.Id);
            orderToUpdate.SetPaymentId(@event.PaymentMethod.Id);

            _logger.CreateLogger(nameof(UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler))
                .LogTrace($"Order with Id: {@event.OrderId} has been successfully updated with a payment method id: { @event.PaymentMethod.Id }");
        }
    }
}
